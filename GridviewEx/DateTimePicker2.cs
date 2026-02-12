using System;
using System.Windows.Forms;  

namespace coms.COMMON.ui
{
    /// <summary>
    /// for DataGridviewEx.DateTimePickerEditingControl
    /// </summary>
    public class DateTimePicker2 : System.Windows.Forms.DateTimePicker   
	{
		private DateTimePickerFormat oldFormat = DateTimePickerFormat.Long;
		private string oldCustomFormat = null;
		private bool bIsNull = false;

        #region Property
        /// <summary>NullAbled</summary>
        public bool AllowNull { get; set; } = true;

        /// <summary>Value Status (null).</summary>
        public bool IsNull { get { return bIsNull; } set { bIsNull = value; } }

        /// <summary>When has value </summary>
        public string DisplayFormat { get; set; } = "yyyy/MM/dd";

        /// <summary>When value is null (" " Empty String).</summary>
        public string NullFormat { get; set; } = " ";

        /// <summary>spinner hour/minus</summary>
        public bool ShowTimeUpDownEx { get; set; } = false;

        /// <summary>èTññÅiìyÅEì˙Åjì¸óÕïsâ¬ÇÃÉtÉâÉO</summary>
        public bool BlockWeekend { get; set; } = false;
        #endregion Property

        public DateTimePicker2() : base()
		{
		}

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
				if (value == DateTime.MinValue)
				{
					if (bIsNull == false) 
					{
						oldFormat = this.Format;
						oldCustomFormat = this.CustomFormat;
						bIsNull = true;
					}

					this.Format = DateTimePickerFormat.Custom;
					this.CustomFormat = " ";
				}
				else 
				{
					if (bIsNull) 
					{
						this.Format = oldFormat;
						this.CustomFormat = oldCustomFormat;
						bIsNull = false;
					}
					base.Value = value;
				}
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
                    if (bIsNull == false)
                    {
                        oldFormat = this.Format;
                        oldCustomFormat = this.CustomFormat;
                        bIsNull = true;
                    }

                    this.Format = DateTimePickerFormat.Custom;
                    this.CustomFormat = " ";
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
                }
            }
        }

        public new DateTime? Date
        {
            get
            {
                if (this.Value2.HasValue) {
                    return this.Value2.Value.Date;
                }
                return null;
            }            
        }

        #region CustomFunctions
        /// <summary>set value with format</summary>
        public void ApplyVisual()
        {
            Format = DateTimePickerFormat.Custom;
            base.CustomFormat = IsNull ? NullFormat : DisplayFormat;
            ShowUpDown = ShowTimeUpDownEx;
        }
        /// <summary>set null value</summary>
        public void ClearToNull()
        {
            if (!AllowNull) return;
            IsNull = true;
            ApplyVisual();
            OnValueChanged(EventArgs.Empty); // notify changed
        }

        /// <summary>set NullAble flag</summary>
        public void SetValueNullable(DateTime? value)
        {
            if (value.HasValue)
            {
                Value = value.Value;
                IsNull = false;
            }
            else
            {
                IsNull = true;
            }
            ApplyVisual();
        }

        /// <summary>get value with nullable.</summary>
        public DateTime? GetValueNullable()
        {
            return IsNull ? (DateTime?)null : Value;
        }
        #endregion CustomFunctions

        #region Event

        protected override void OnCloseUp(EventArgs eventargs)
		{
            base.OnCloseUp(eventargs);

            //if (Control.MouseButtons == MouseButtons.None) 
			//{
				if (AllowNull && IsNull) 
				{
					//this.Format = oldFormat;
					//this.CustomFormat = oldCustomFormat;
                    IsNull = false;
                    ApplyVisual();
                }
			//}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);

            if (AllowNull && (e.KeyCode == Keys.Delete || (e.Control && e.KeyCode == Keys.Back)))
            {
                //this.Value = DateTime.MinValue;
                //bIsNull = true;
                ClearToNull();
                e.Handled = true;
                return;
            }
		}

        protected override void OnValueChanged(EventArgs eventargs)
        {
            // èTññÅiìyÅEì˙Åjì¸óÕïsâ¬ÇÃèÍçáÇÕã‡ójì˙Ç…ã≠êßïœä∑
            if (BlockWeekend && !IsNull)
            {
                if (Value.DayOfWeek == DayOfWeek.Saturday)
                    Value = Value.AddDays(-1); // ã‡ójì˙
                else if (Value.DayOfWeek == DayOfWeek.Sunday)
                    Value = Value.AddDays(-2); // ã‡ójì˙
            }
            base.OnValueChanged(eventargs);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            ApplyVisual();
        }
        #endregion Event

    }
}
