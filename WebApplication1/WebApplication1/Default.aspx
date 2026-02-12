<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        :root {
            --table-top1-head-text-blue: #094B94;
            --table-top1-border-blue: #84B9E5;
            --table-top1-border-light-blue: #DEEBF7;
            --table-top1-text-green: #4797D7;
            --page-title: #5DB2E1;
            --table-top1-text-yl: #BF9000;
            --table-top1-text-red: #FA6D57;
            --text-gray: #7F7F7F;
            --table-top1-border-gray: #D9D9D9;
            --table-top-border-pink: #FEDFDA;
            --tabu-sub-summary-bg: #FAECEC;
        }

        .m07_page_top {
            padding-left: 5px;
            color: var(--page-title);
            border-left: solid 1px;
        }
        /*  top area */
        .m07_div_flex {
            display: flex;
        }
        .m07_div_flex td {
            height: 27px;
        }
        /* table top */
        .m07_table_top {
            font-size: 12px;
            border-collapse: collapse;
            color: black;
        }
        .m07_table_top .col120 { width: 120px; }
        .m07_table_top .col150 { width: 150px; }
        .m07_table_top .col90 { width: 90px; }
        .m07_table_top .col100 { width: 100px; }
        .m07_table_top .col180 { width: 180px; }
        .m07_table_top .col200 { width: 200px; }
        .m07_table_top .col290 { width: 290px; }
        .m07_table_top .col400 { width: 400px; }
        .m07_table_top .header { text-align: center; }
        .m07_table_top .header_red { color: white; background-color: #FA6D57; }
        .m07_table_top .header_gray { background-color: #F2F2F2; }
        .m07_table_top .header_blue { 
            background-color: var(--table-top1-border-light-blue); 
            color: var(--table-top1-head-text-blue); 
        }
        .m07_table_top .header_green { 
            background-color: var(--table-top1-text-green); 
            color: white;
        }
        .m07_table_top .top_left {
            border-left: solid 2px var(--table-top1-border-gray);
            border-top: solid 2px var(--table-top1-border-gray);
            border-bottom: solid 1px var(--table-top1-border-gray);
        }
        .m07_table_top .top_mid {
            border-top: solid 2px var(--table-top1-border-gray);
            border-left: solid 1px var(--table-top1-border-gray);
        }
        .m07_table_top .top_right {
            border-right: solid 2px var(--table-top1-border-gray);
            border-top: solid 2px var(--table-top1-border-gray);
            border-left: solid 1px var(--table-top1-border-gray);
        }
        .m07_table_top .mid_left {
            border-left: solid 2px var(--table-top1-border-gray);
            border-bottom: solid 1px var(--table-top1-border-gray);
        }
        .m07_table_top .mid_mid {
            border-top: solid 1px var(--table-top1-border-gray);
            border-left: solid 1px var(--table-top1-border-gray);
        }
        .m07_table_top .mid_right {
            border-right: solid 2px var(--table-top1-border-gray);
            border-top: solid 1px var(--table-top1-border-gray);
            border-left: solid 1px var(--table-top1-border-gray);
        }
        .m07_table_top .bottom_left {
            border-left: solid 2px var(--table-top1-border-gray);
            border-bottom: solid 2px var(--table-top1-border-gray);
        }
        .m07_table_top .bottom_mid {
            border-top: solid 1px var(--table-top1-border-gray);
            border-left: solid 1px var(--table-top1-border-gray);
            border-bottom: solid 2px var(--table-top1-border-gray);
        }
        .m07_table_top .bottom_right {
            border-right: solid 2px var(--table-top1-border-gray);
            border-top: solid 1px var(--table-top1-border-gray);
            border-left: solid 1px var(--table-top1-border-gray);
            border-bottom: solid 2px var(--table-top1-border-gray);
        }
        /* table 1 */
        .m07_table_top .tab1_cell_top {
            border-right: solid 2px var(--table-top1-border-blue) !important;
            border-top: solid 2px var(--table-top1-border-blue) !important;
            border-left: solid 2px var(--table-top1-border-blue) !important;
        }
        .m07_table_top .tab1_cell_mid {
            border-right: solid 2px var(--table-top1-border-blue) !important;
            border-left: solid 2px var(--table-top1-border-blue) !important;
        }
        .m07_table_top .tab1_cell_bottom {
            border-right: solid 2px var(--table-top1-border-blue) !important;
            border-bottom: solid 2px var(--table-top1-border-blue) !important;
            border-left: solid 2px var(--table-top1-border-blue) !important;
        }
        /* table 2 */
        .m07_table_top .tab2_top_all {
            border-left: solid 2px var(--table-top-border-pink) !important;
            border-top: solid 2px var(--table-top-border-pink) !important;
            border-right: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab2_mid_left {
            border-left: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab2_mid_right {
            border-right: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab2_bottom_left {
            border-left: solid 2px var(--table-top-border-pink) !important;
            border-bottom: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab2_bottom_right {
            border-right: solid 2px var(--table-top-border-pink) !important;
            border-bottom: solid 2px var(--table-top-border-pink) !important;
        }
        .div_top_display { z-index: 2; }
        /* table 4 & 5 */
        .div_bottom_margin { margin-top: -17px; z-index: 1; }
        .m07_table_top .td_separate { width: 10px; height: 100% !important; }
        .td_div_col { width: 290px; height: 100%; line-height: 27px; }
        .m07_table_top .tab4_top_all {
            border: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab4_mid_left {
            border-left: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab4_mid_right {
            border-right: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab4_bottom_left {
            border-left: solid 2px var(--table-top-border-pink) !important;
            border-bottom: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab4_bottom_mid {
            border-bottom: solid 2px var(--table-top-border-pink) !important;
        }
        .m07_table_top .tab4_bottom_right {
            border-right: solid 2px var(--table-top-border-pink) !important;
            border-bottom: solid 2px var(--table-top-border-pink) !important;
        }

        .m07_table_top .tab5_top_left {
            border-left: solid 2px var(--table-top1-border-light-blue) !important;
            border-top: solid 2px var(--table-top1-border-light-blue) !important;
        }
        .m07_table_top .tab5_top_right {
            border-right: solid 2px var(--table-top1-border-light-blue) !important;
            border-top: solid 2px var(--table-top1-border-light-blue) !important;
        }
        .m07_table_top .tab5_mid_left {
            border-left: solid 2px var(--table-top1-border-light-blue) !important;
        }
        .m07_table_top .tab5_mid_right {
            border-right: solid 2px var(--table-top1-border-light-blue) !important;
        }
        .m07_table_top .tab5_bottom_left {
            border-left: solid 2px var(--table-top1-border-light-blue) !important;
            border-bottom: solid 2px var(--table-top1-border-light-blue) !important;
        }
        .m07_table_top .tab5_bottom_right {
            border-right: solid 2px var(--table-top1-border-light-blue) !important;
            border-bottom: solid 2px var(--table-top1-border-light-blue) !important;
        }
        /*text color*/
        .m07_tab_top_text_left { 
            text-align: start;
            padding-left: 5px;
        }
        .m07_tab_top_text_right { 
            text-align: end;
            padding-right: 5px;
        }
        .m07_tab_top_text_center { 
            text-align: center;
        }
        .m07_tab_top_text_green { 
            color: var(--table-top1-text-green) !important;
        }
        .m07_tab_top_text_red { 
            color: var(--table-top1-text-red) !important;
        }
        .m07_tab_top_text_yellow { 
            color: var(--table-top1-text-yl) !important;
        }
        .m07_tab_top_text_bold { 
            font-weight: bold;
        }
    </style>
    <div class="m07_page_top">
        諸費用明細
    </div>
    <div>
        <div class="m07_div_flex div_top_display">
            <table class="m07_table_top">
                <tbody>
                    <tr>
                        <td class="col120 header top_left header_red">諸費用(円)</td>
                        <td class="col120 header top_mid header_gray">予定</td>
                        <td class="col120 header top_mid tab1_cell_top header_blue">確定済</td>
                        <td class="col120 header top_right">未確定</td>
                    </tr>
                    <tr>
                        <td class="col120 mid_left m07_tab_top_text_left">合計額</td>
                        <td class="col120 mid_mid m07_tab_top_text_right m07_tab_top_text_yellow m07_tab_top_text_bold">2,000,000</td>
                        <td class="col120 mid_mid tab1_cell_mid m07_tab_top_text_right m07_tab_top_text_bold">1,790,000</td>
                        <td class="col120 mid_right m07_tab_top_text_right">200,000</td>
                    </tr>
                    <tr>
                        <td class="col120 mid_left m07_tab_top_text_red m07_tab_top_text_left">サービス</td>
                        <td class="col120 mid_mid m07_tab_top_text_right m07_tab_top_text_red m07_tab_top_text_bold">500,000</td>
                        <td class="col120 mid_mid tab1_cell_mid m07_tab_top_text_right m07_tab_top_text_red m07_tab_top_text_bold">500,000</td>
                        <td class="col120 mid_right m07_tab_top_text_right">100</td>
                    </tr>
                    <tr>
                        <td class="col120 bottom_left m07_tab_top_text_green m07_tab_top_text_left">顧客負担額</td>
                        <td class="col120 bottom_mid m07_tab_top_text_right m07_tab_top_text_green m07_tab_top_text_bold">1,500,000</td>
                        <td class="col120 bottom_mid tab1_cell_bottom m07_tab_top_text_right m07_tab_top_text_green m07_tab_top_text_bold">1,290,000</td>
                        <td class="col120 bottom_right m07_tab_top_text_right">a</td>
                    </tr>
                    <tr class="table_top1_tr_hide_botom">
                        <td class="col120 m07_tab_top_text_left m07_tab_top_text_green"></td>
                        <td class="col120 m07_tab_top_text_right m07_tab_top_text_green m07_tab_top_text_bold"></td>
                        <td class="col120 m07_tab_top_text_right m07_tab_top_text_green m07_tab_top_text_bold item_last"></td>
                        <td class="col120 m07_tab_top_text_right"></td>
                    </tr>
                </tbody>
            </table>
            <div style="width: 10px;"></div>
            <table class="m07_table_top">
                <tbody>
                    <tr>
                        <td class="col290 m07_tab_top_text_left top_left tab2_top_all header_red" colspan="2">諸費用請求額(①除く)</td>
                    </tr>
                    <tr>
                        <td class="col90 mid_left tab2_mid_left m07_tab_top_text_left">請求日</td>
                        <td class="col200 mid_right tab2_mid_right m07_tab_top_text_right">200,000</td>
                    </tr>
                    <tr>
                        <td class="col90 mid_left tab2_mid_left m07_tab_top_text_left">入金予定日</td>
                        <td class="col200 mid_right tab2_mid_right m07_tab_top_text_right">100</td>
                    </tr>
                    <tr>
                        <td class="col90 bottom_left tab2_bottom_left m07_tab_top_text_left">入金日</td>
                        <td class="col200 bottom_right tab2_bottom_right m07_tab_top_text_right">a</td>
                    </tr>
                    <tr class="table_top1_tr_hide_botom">
                        <td class="col90 m07_tab_top_text_left m07_tab_top_text_green"></td>
                        <td class="col200 m07_tab_top_text_right"></td>
                    </tr>
                </tbody>
            </table>
            <div style="width: 20px;"></div>
            <table class="m07_table_top">
                <tbody>
                    <tr>
                        <td class="col90 header top_left">諸費用合計</td>
                        <td class="col120 top_right m07_tab_top_text_right">1,790,000</td>
                        <td class="col150"></td>
                    </tr>
                    <tr>
                        <td class="col90 mid_left header">振替額合計</td>
                        <td class="col120 mid_right m07_tab_top_text_right">1,790,000</td>
                        <td class="col150"></td>
                    </tr>
                    <tr>
                        <td class="col90 mid_left header_red m07_tab_top_text_left">諸費用残高</td>
                        <td class="col120 mid_right m07_tab_top_text_right m07_tab_top_text_bold header_red">500,000</td>
                        <td class="col150"></td>
                    </tr>
                    <tr>
                        <td class="col90 mid_left header">途中返金合計</td>
                        <td class="col120 mid_right m07_tab_top_text_right">1,290,000</td>
                        <td class="col150"></td>
                    </tr>
                    <tr class="table_top1_tr_hide_botom">
                        <td class="col90 header bottom_left">途中請求合計</td>
                        <td class="col120 m07_tab_top_text_right bottom_right">1,790,000</td>
                        <td class="col150"></td>
                </tbody>
            </table>
        </div>
        <div class="m07_div_flex div_bottom_margin">
            <table class="m07_table_top">
                <tbody>
                    <tr>
                        <td>
                            <table>
                                <tbody>
                                    <tr>
                                        <td colspan="4"><div class="td_div_col header_red m07_tab_top_text_left">諸費用充当額①</div></td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" class="m07_tab_top_text_left tab4_top_all">事前受領金明細</td>
                                    </tr>
                                    <tr>
                                        <td class="col150 mid_left tab4_mid_left m07_tab_top_text_left">手付金</td>
                                        <td class="col120 mid_mid m07_tab_top_text_right">500,000</td>
                                        <td class="col90 mid_mid m07_tab_top_text_left">融資分</td>
                                        <td class="col120 mid_right m07_tab_top_text_right tab4_mid_right">100</td>
                                    </tr>
                                    <tr>
                                        <td class="col150 mid_left m07_tab_top_text_left tab4_mid_left">内金</td>
                                        <td class="col120 mid_mid m07_tab_top_text_right"></td>
                                        <td class="col90 mid_mid m07_tab_top_text_left">買取充当額</td>
                                        <td class="col120 mid_right m07_tab_top_text_right tab4_mid_right"></td>
                                    </tr>
                                    <tr>
                                        <td class="col150 bottom_left tab4_bottom_left m07_tab_top_text_left">残代金のうち現金</td>
                                        <td class="col120 bottom_mid tab4_bottom_mid m07_tab_top_text_right">1,500,000</td>
                                        <td class="col90 bottom_mid tab4_bottom_mid m07_tab_top_text_left">余剰金</td>
                                        <td class="col120 bottom_right tab4_bottom_right m07_tab_top_text_right">a</td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td class="td_separate"></td>
                        <td>
                            <table>
                                <tbody>
                                    <tr>
                                        <td colspan="2"><div class="td_div_col header_green m07_tab_top_text_left">精算返金額</div></td>
                                    </tr>
                                    <tr>
                                        <td class="col120 mid_left m07_tab_top_text_center tab5_top_left">支払予定日</td>
                                        <td class="col400 mid_right m07_tab_top_text_left tab5_top_right"></td>
                                    </tr>
                                    <tr>
                                        <td class="col120 mid_left tab5_mid_left m07_tab_top_text_center">出金又は振替</td>
                                        <td class="col400 mid_right m07_tab_top_text_right tab5_mid_right">100</td>
                                    </tr>
                                    <tr>
                                        <td class="col120 mid_left m07_tab_top_text_center tab5_mid_left">出金口座</td>
                                        <td class="col400 mid_right m07_tab_top_text_right tab5_mid_right"></td>
                                    </tr>
                                    <tr>
                                        <td class="col120 bottom_left tab5_bottom_left m07_tab_top_text_center">支払口座</td>
                                        <td class="col400 bottom_right tab5_bottom_right m07_tab_top_text_right">a</td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div>

    </div>
    <div>

    </div>
    <div>

    </div>
</asp:Content>
