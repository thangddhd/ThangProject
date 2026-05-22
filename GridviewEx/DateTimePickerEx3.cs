using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace coms.COMMON.ui
{
    public class DateTimePickerEx3 : System.Windows.Forms.DateTimePicker
    {
        private DateTimePickerFormat oldFormat = DateTimePickerFormat.Long;
        private string oldCustomFormat = null;
        private bool bIsNull = false;

        // Overlay textbox to support selection / copy / paste
        private TextBox _overlayTextBox;

        // approximate width reserved for the drop-down button (so we don't cover it)
        private const int DropDownButtonWidth = 20;

        // suppress flag to avoid reacting to programmatic changes of overlay.Text
        private bool _suppressOverlayTextChanged = false;

        // DisplayBackColor logic
        private Color _displayBackColor = Color.Empty;
        private bool _displayBackColorIsCustom = false;

        public DateTimePickerEx3() : base()
        {
            // default display back color follows the control BackColor unless user overrides via DisplayBackColor property
            _displayBackColor = this.BackColor;
            _displayBackColorIsCustom = false;

            InitializeOverlay();
        }

        /// <summary>
        /// Background color used for the visible text area (overlay) when the picker is not dropped down.
        /// If not set explicitly, this follows the control BackColor.
        /// </summary>
        [Category("Appearance")]
        [Description("Background color used for the visible text area when the calendar is not dropped down.")]
        public Color DisplayBackColor
        {
            get { return _displayBackColor; }
            set
            {
                _displayBackColor = value;
                _displayBackColorIsCustom = true;
                if (_overlayTextBox != null)
                    _overlayTextBox.BackColor = _displayBackColor;
                Invalidate();
            }
        }

        // Compatibility helper: works on all .NET versions
        private static bool IsNullOrWhiteSpaceCompat(string s)
        {
            return s == null || s.Trim().Length == 0;
        }

        private void InitializeOverlay()
        {
            _overlayTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = _displayBackColor,
                ForeColor = this.ForeColor,
                Font = this.Font,
                Visible = true,
                Multiline = true,       // allow vertical space and EM_SETRECT to center text
                ReadOnly = false,
                TabStop = false,        // don't take tab focus away from original control
                Cursor = Cursors.IBeam
            };

            // Place overlay as child of this control
            this.Controls.Add(_overlayTextBox);

            // Events
            _overlayTextBox.KeyDown += OverlayTextBox_KeyDown;
            _overlayTextBox.LostFocus += OverlayTextBox_LostFocus;
            _overlayTextBox.TextChanged += OverlayTextBox_TextChanged;
            _overlayTextBox.KeyPress += OverlayTextBox_KeyPress;
            this.Resize += DateTimePicker_Resize;
            this.FontChanged += DateTimePicker_FontChanged;
            this.BackColorChanged += DateTimePicker_BackColorChanged;
            this.ForeColorChanged += DateTimePicker_ForeColorChanged;
            UpdateOverlayBounds();

            // initialize overlay text consistent with current Value / null state
            SyncOverlayTextFromValue();
        }

        private void DateTimePicker_BackColorChanged(object sender, EventArgs e)
        {
            // If the user didn't explicitly set DisplayBackColor, follow BackColor changes.
            if (!_displayBackColorIsCustom)
            {
                _displayBackColor = this.BackColor;
                if (_overlayTextBox != null)
                    _overlayTextBox.BackColor = _displayBackColor;
            }
        }

        private void DateTimePicker_ForeColorChanged(object sender, EventArgs e)
        {
            if (_overlayTextBox != null)
                _overlayTextBox.ForeColor = this.ForeColor;
        }

        private void DateTimePicker_FontChanged(object sender, EventArgs e)
        {
            if (_overlayTextBox != null)
            {
                _overlayTextBox.Font = this.Font;
                UpdateOverlayBounds();
            }
        }

        private void DateTimePicker_Resize(object sender, EventArgs e)
        {
            UpdateOverlayBounds();
        }

        private void UpdateOverlayBounds()
        {
            if (_overlayTextBox == null) return;

            // leave right area for drop-down button
            int w = Math.Max(0, this.Width - DropDownButtonWidth);
            int h = this.Height;
            // give small vertical padding so text is not glued to control edges.
            int left = 2;
            int top = 1;
            _overlayTextBox.SetBounds(left, top, Math.Max(0, w - 4), Math.Max(0, h - 2));
            UpdateOverlayFormatting();
        }

        // P/Invoke for setting formatting rectangle of TextBox (so we can vertically center text)
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int left, top, right, bottom; }

        private const int EM_SETRECTNP = 0xB3;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lParam);

        private void UpdateOverlayFormatting()
        {
            if (_overlayTextBox == null || !_overlayTextBox.IsHandleCreated) return;

            // client height and text height
            int clientH = _overlayTextBox.ClientSize.Height;
            int textH = TextRenderer.MeasureText("Mg", _overlayTextBox.Font).Height; // "Mg" to approximate ascent+descent
            int topPadding = Math.Max(0, (clientH - textH) / 2);

            RECT rc = new RECT
            {
                left = 0,
                top = topPadding,
                right = _overlayTextBox.ClientSize.Width,
                bottom = _overlayTextBox.ClientSize.Height
            };

            try
            {
                SendMessage(_overlayTextBox.Handle, EM_SETRECTNP, IntPtr.Zero, ref rc);
            }
            catch
            {
                // ignore on failure; control will just use default top alignment
            }
        }

        private void OverlayTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Delete key -> clear
            if (e.KeyCode == Keys.Delete && _overlayTextBox.SelectionLength == _overlayTextBox.Text.Length)
            {
                // Only clear if all text is selected (traditional Delete behavior)
                SetNullValue();
                e.Handled = true;
                return;
            }

            // Ctrl+V (paste) - allow and validate on blur
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Let paste happen normally, validation will occur on blur
                return;
            }

            // Optional: Filter allowed characters while typing
            // Allow: digits, /, -, space, :, backspace, delete, arrow keys, home, end
            bool isControlKey = e.Control;
            bool isAllowedKey =
                e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Home ||
                e.KeyCode == Keys.End ||
                e.KeyCode == Keys.Tab;

            if (isControlKey || isAllowedKey)
            {
                // Allow control operations
                return;
            }

            // If not a control key, check if it's an allowed character
            // This is handled better in KeyPress event - see below
        }

        private void OverlayTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suppressOverlayTextChanged) return;

            // Remove immediate validation - just allow typing
            // Validation will happen on LostFocus only

            // Optional: Visual feedback for invalid input
            string s = _overlayTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(s))
            {
                // Empty is valid (will clear on blur)
                _overlayTextBox.BackColor = _displayBackColor;
                return;
            }

            // Show visual hint if invalid (optional - remove if you don't want this)
            if (TryParseTextToDate(s, out DateTime parsed))
            {
                _overlayTextBox.BackColor = _displayBackColor;
            }
            else
            {
                // Light red tint to indicate "not yet valid" while typing
                _overlayTextBox.BackColor = Color.FromArgb(255, 240, 240);
            }
        }

        private void OverlayTextBox_LostFocus(object sender, EventArgs e)
        {
            // Reset background color
            _overlayTextBox.BackColor = _displayBackColor;

            // When user leaves the field:
            // - Empty text → clear/null
            // - Valid date → apply
            // - Invalid date → clear/null (your requirement)

            string s = _overlayTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(s))
            {
                SetNullValue();
                return;
            }

            if (TryParseTextToDate(s, out DateTime parsed))
            {
                // Valid date - apply it 
                this.Value = parsed; // set DatetimePickerValue to same textbox value
            }
            else
            {
                // Invalid date - clear it (as per your requirement)
                SetNullValue();
            }

            // Ensure no selection remains
            _overlayTextBox.SelectionLength = 0;
        }

        private void SetNullValue()
        {
            // keep existing behaviour: show blank state and mark bIsNull,
            // but DO NOT assign base.Value = DateTime.MinValue (invalid for DateTimePicker).
            _suppressOverlayTextChanged = true;
            try
            {
                // store old formats if not already stored
                if (!bIsNull)
                {
                    oldFormat = this.Format;
                    oldCustomFormat = this.CustomFormat;
                }

                bIsNull = true;

                // show blank state (use a single space custom format to render empty)
                this.Format = DateTimePickerFormat.Custom;
                this.CustomFormat = " ";
                if (_overlayTextBox != null)
                    _overlayTextBox.Text = "";
            }
            finally
            {
                _suppressOverlayTextChanged = false;
            }
        }

        private void SyncOverlayTextFromValue()
        {
            if (_overlayTextBox == null) return;

            _suppressOverlayTextChanged = true;
            try
            {
                if (bIsNull)
                {
                    _overlayTextBox.Text = "";
                }
                else
                {
                    // display according to current format/custom format
                    string fmt = GetDisplayFormatString();
                    try
                    {
                        _overlayTextBox.Text = base.Value.ToString(fmt);
                    }
                    catch
                    {
                        // fallback
                        _overlayTextBox.Text = base.Value.ToShortDateString();
                    }
                }

                // ensure formatting rectangle is updated and remove selection
                UpdateOverlayFormatting();
                _overlayTextBox.SelectionLength = 0;
            }
            finally
            {
                _suppressOverlayTextChanged = false;
            }
        }

        private string GetDisplayFormatString()
        {
            if (this.Format == DateTimePickerFormat.Custom && !IsNullOrWhiteSpaceCompat(this.CustomFormat))
            {
                return this.CustomFormat;
            }
            if (this.Format == DateTimePickerFormat.Long)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
            }
            if (this.Format == DateTimePickerFormat.Short)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            }
            if (this.Format == DateTimePickerFormat.Time)
            {
                // use short time pattern for compactness
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
            }
            // fallback
            return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        private bool TryParseTextToDate2(string text, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrEmpty(text))
                return false;

            var culture = CultureInfo.CurrentCulture;

            // Build a list of candidate formats to try (in order).
            var candidateFormats = new System.Collections.Generic.List<string>();

            // If current format is Custom and contains a real pattern, try it first
            if (this.Format == DateTimePickerFormat.Custom && !IsNullOrWhiteSpaceCompat(this.CustomFormat))
            {
                candidateFormats.Add(this.CustomFormat);
            }
            // If we have an oldCustomFormat saved (the one before we set blank), try it too.
            if (!IsNullOrWhiteSpaceCompat(oldCustomFormat))
            {
                candidateFormats.Add(oldCustomFormat);
            }

            // Add culture patterns depending on current Format
            if (this.Format == DateTimePickerFormat.Long)
            {
                candidateFormats.Add(culture.DateTimeFormat.LongDatePattern);
            }
            else if (this.Format == DateTimePickerFormat.Short)
            {
                candidateFormats.Add(culture.DateTimeFormat.ShortDatePattern);
            }
            else if (this.Format == DateTimePickerFormat.Time)
            {
                candidateFormats.Add(culture.DateTimeFormat.ShortTimePattern);
            }

            // Always add the common short/long patterns as fallback
            candidateFormats.Add(culture.DateTimeFormat.ShortDatePattern);
            candidateFormats.Add(culture.DateTimeFormat.LongDatePattern);

            // Try parsing with exact formats (safe-guard against bad format strings)
            try
            {
                // Remove duplicates and whitespace-only entries
                var formats = candidateFormats
                    .Where(f => !IsNullOrWhiteSpaceCompat(f))
                    .Distinct()
                    .ToArray();

                if (formats.Length > 0)
                {
                    if (DateTime.TryParseExact(text, formats, culture, DateTimeStyles.None, out result))
                        return true;
                }
            }
            catch (FormatException)
            {
                // If any format string was invalid, ignore and fall back to flexible parsing below.
            }

            // Finally try general parse (more forgiving)
            if (DateTime.TryParse(text, culture, DateTimeStyles.None, out result))
                return true;

            return false;
        }

        // keep your original Value property semantics (nullable behaviour via DateTime.MinValue)
        public new DateTime Value
        {
            get
            {
                if (bIsNull)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return base.Value;
                }
            }
            set
            {
                if (value <= this.MinDate || value > this.MaxDate)
                {
                    // switch to "null/blank" state without assigning base.Value to DateTime.MinValue
                    if (!bIsNull)
                    {
                        oldFormat = this.Format;
                        oldCustomFormat = this.CustomFormat;
                        bIsNull = true;
                    }
                    //When datetime is MinDate show empty 
                    this.Format = DateTimePickerFormat.Custom;
                    this.CustomFormat = " ";

                    if (_overlayTextBox != null)
                    {
                        _suppressOverlayTextChanged = true;
                        _overlayTextBox.Text = "";
                        _suppressOverlayTextChanged = false;
                    }
                }
                else
                {
                    if (bIsNull)
                    {
                        // restoring previous format when coming from null state
                        this.Format = oldFormat;
                        this.CustomFormat = oldCustomFormat;
                        bIsNull = false;
                    }
                    base.Value = value;
                    // update overlay to show formatted text
                    SyncOverlayTextFromValue();
                }

                // do NOT force focus here to avoid trapping focus (OnCloseUp will handle focus if needed)
            }
        }

        public new DateTime? Value2
        {
            get
            {
                if (bIsNull)
                {
                    return null;
                }
                else
                {
                    return base.Value;
                }
            }
            set
            {
                if (value == DateTime.MinValue || value == null)
                {
                    if (!bIsNull)
                    {
                        oldFormat = this.Format;
                        oldCustomFormat = this.CustomFormat;
                        bIsNull = true;
                    }

                    this.Format = DateTimePickerFormat.Custom;
                    this.CustomFormat = " ";
                    _suppressOverlayTextChanged = true;
                    if (_overlayTextBox != null) _overlayTextBox.Text = "";
                    _suppressOverlayTextChanged = false;

                    // Do NOT set base.Value to DateTime.MinValue
                }
                else
                {
                    if (bIsNull)
                    {
                        this.Format = oldFormat;
                        this.CustomFormat = oldCustomFormat;
                        bIsNull = false;
                    }
                    base.Value = value.Value;
                    SyncOverlayTextFromValue();
                }

                // do NOT focus overlay here
            }
        }

        public new DateTime? Date
        {
            get
            {
                if (this.Value2.HasValue)
                {
                    return this.Value2.Value.Date;
                }
                return null;
            }
        }

        // When the calendar dropdown opens, hide the overlay so the popup/calendar receives interaction normally.
        protected override void OnDropDown(EventArgs e)
        {
            // hide overlay to avoid it interfering with the calendar popup interactions
            if (_overlayTextBox != null)
                _overlayTextBox.Visible = false;

            base.OnDropDown(e);
        }

        protected override void OnCloseUp(EventArgs eventargs)
        {
            // If we had previously used the blank state, restore formats (same logic as before)
            if (Control.MouseButtons == MouseButtons.None)
            {
                if (bIsNull)
                {
                    this.Format = oldFormat;
                    this.CustomFormat = oldCustomFormat;
                    bIsNull = false;
                }
            }

            // Ensure overlay text matches newly selected value when calendar closed
            SyncOverlayTextFromValue();

            // Show overlay back. Post a focus action to the message queue so we don't re-enter focus changes synchronously.
            if (_overlayTextBox != null)
            {
                _overlayTextBox.Visible = true;
                // Use BeginInvoke to set focus after current event completes (avoids reentrancy)
                this.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        if (_overlayTextBox.CanFocus)
                        {
                            _overlayTextBox.Focus();
                            _overlayTextBox.SelectionLength = 0;
                        }
                    }
                    catch { }
                }));
            }

            base.OnCloseUp(eventargs);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // We still want Delete key to clear when the picker itself has focus
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Delete)
                this.Value = DateTime.MinValue;
        }

        // Keep overlay in sync if user programatically changes Format/CustomFormat
        public new DateTimePickerFormat Format
        {
            get => base.Format;
            set
            {
                base.Format = value;
                SyncOverlayTextFromValue();
            }
        }

        public new string CustomFormat
        {
            get => base.CustomFormat;
            set
            {
                base.CustomFormat = value;
                SyncOverlayTextFromValue();
            }
        }

        private void OverlayTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow: digits, '/', '-', ':', space, and control characters (backspace, etc.)
            if (char.IsControl(e.KeyChar))
            {
                // Allow all control characters (backspace, delete, etc.)
                return;
            }

            if (char.IsDigit(e.KeyChar) ||
                e.KeyChar == '/' ||
                e.KeyChar == '-' ||
                e.KeyChar == ':' ||
                e.KeyChar == ' ')
            {
                // Allow valid date/time characters
                return;
            }

            // Block all other characters
            e.Handled = true;
        }

        private bool TryParseTextToDate(string text, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrEmpty(text))
                return false;

            var culture = CultureInfo.CurrentCulture;

            // Build candidate formats with BOTH strict and flexible variants
            var candidateFormats = new System.Collections.Generic.List<string>();

            // Add current custom format
            if (this.Format == DateTimePickerFormat.Custom && !IsNullOrWhiteSpaceCompat(this.CustomFormat))
            {
                candidateFormats.Add(this.CustomFormat);
                // Add flexible variant (MM→M, dd→d, etc.)
                candidateFormats.Add(MakeFlexibleFormat(this.CustomFormat));
            }

            // Add old custom format
            if (!IsNullOrWhiteSpaceCompat(oldCustomFormat))
            {
                candidateFormats.Add(oldCustomFormat);
                candidateFormats.Add(MakeFlexibleFormat(oldCustomFormat));
            }

            // Add culture patterns based on current Format
            if (this.Format == DateTimePickerFormat.Long)
            {
                candidateFormats.Add(culture.DateTimeFormat.LongDatePattern);
                candidateFormats.Add(MakeFlexibleFormat(culture.DateTimeFormat.LongDatePattern));
            }
            else if (this.Format == DateTimePickerFormat.Short)
            {
                candidateFormats.Add(culture.DateTimeFormat.ShortDatePattern);
                candidateFormats.Add(MakeFlexibleFormat(culture.DateTimeFormat.ShortDatePattern));
            }
            else if (this.Format == DateTimePickerFormat.Time)
            {
                candidateFormats.Add(culture.DateTimeFormat.ShortTimePattern);
                candidateFormats.Add(MakeFlexibleFormat(culture.DateTimeFormat.ShortTimePattern));
            }

            // Always add common patterns as fallback
            candidateFormats.Add(culture.DateTimeFormat.ShortDatePattern);
            candidateFormats.Add(MakeFlexibleFormat(culture.DateTimeFormat.ShortDatePattern));
            candidateFormats.Add(culture.DateTimeFormat.LongDatePattern);
            candidateFormats.Add(MakeFlexibleFormat(culture.DateTimeFormat.LongDatePattern));

            // Remove duplicates and whitespace-only entries
            var formats = candidateFormats
                .Where(f => !IsNullOrWhiteSpaceCompat(f))
                .Distinct()
                .ToArray();

            try
            {
                if (formats.Length > 0)
                {
                    // Try parsing with exact formats - AllowWhiteSpaces is lenient enough
                    if (DateTime.TryParseExact(text, formats, culture,
                        DateTimeStyles.AllowWhiteSpaces, out result))
                    {
                        return true;
                    }
                }
            }
            catch (FormatException)
            {
                // Invalid format, fall through
            }

            // Do NOT use flexible DateTime.TryParse() - it's too lenient
            return false;
        }

        // New helper method: convert strict format to flexible format
        // Example: "MM/dd/yyyy" → "M/d/yyyy"
        private string MakeFlexibleFormat(string format)
        {
            if (string.IsNullOrEmpty(format))
                return format;

            // Replace double-character patterns with single-character (allowing 1 or 2 digits)
            return format
                .Replace("MM", "M")
                .Replace("dd", "d")
                .Replace("HH", "H")
                .Replace("hh", "h")
                .Replace("mm", "m")
                .Replace("ss", "s");
        }
    }
}
