<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="Asm_NoSolve_New.aspx.cs" Inherits="dek_erpvis_v2.pages.dp_PM.Asm_NoSolve_New" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>異常排程編號 | <%= WebUtils.GetAppSettings("Company_Name") %></title>
    <%=color %>

    <link href="../../Content/Default.css" rel="stylesheet" />
    <link href="../../Content/table.css" rel="stylesheet" />
    <link href="../../Content/dp_PM/Asm_LineOverView20220324.css" rel="stylesheet" />
    <style>
        #testmodal2 {
            padding: 5px 20px;
        }

        @media screen and (max-width:768px) {
            #myModalLabel2 i {
                margin-right: 10px;
            }

            #testmodal2 {
                padding: 0;
            }

                #testmodal2 div.form-group h5 {
                    width: 35%;
                    display: inline-block;
                }

                #testmodal2 div.form-group div,
                #testmodal2 div.form-group select,
                #testmodal2 div.form-group fieldset {
                    width: 63%;
                    display: inline-block;
                }

                    #testmodal2 div.form-group:last-child fieldset,
                    #testmodal2 div.form-group fieldset select {
                        width: 100%;
                    }

                #testmodal2 div.form-group fieldset {
                    height: 45px;
                    line-height: 45px;
                }

            #ContentPlaceHolder1_RadioButtonList_select_type {
                width: 100%;
            }

            #testmodal2 div.form-group fieldset tbody tr {
                width: 32%;
                display: inline-block;
            }

                #testmodal2 div.form-group fieldset tbody tr input {
                    margin-right: 5px;
                }

            .modal-footer .btn-primary, #btncheck {
                width: 47%;
                display: inline-block;
            }

            #exampleModal .modal-body {
                padding-bottom: 0;
            }

            #exampleModal .modal-footer {
                width: 100%;
                padding: 10px;
                margin-bottom: 10px;
            }
        }


          /*手機*/
        @media screen and (max-width: 765px) {

            .tooltip-inner {
                max-width: 200px;
                /* If max-width does not work, try using width instead */
                width: 200px;
                 filter: alpha(opacity=0);
            }

  
            .tooltip.in{opacity:1!important; }
        }
        /*電腦*/
        @media screen and (min-width: 765px) {

            .tooltip-inner {
                max-width: 250px;
                /* If max-width does not work, try using width instead */
                width: 250px;
				        font-size: 18px;
                         filter: alpha(opacity=0);
            }

            .tooltip.in{opacity:1!important;}
        }
		
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="right_col" role="main">
        <!-----------------title------------------>
        <ol class="breadcrumb_">
            <li><u><a href="../index.aspx">首頁</a></u></li>
            <li><u><a href="../SYS_CONTROL/dp_fuclist.aspx?dp=PMD">生產部</a></u></li>

        </ol>
        <!-----------------title------------------>
        <div class="row tile_time">
            <h1 class="text-center _mdTitle" style="width: 100%; margin-bottom: 15px"><b>異常排程編號</b></h1>
            <h3 class="text-center _xsTitle" style="width: 100%; margin-bottom: 15px"><b>異常排程編號</b></h3>
        </div>
       
        <!-- top tiles -->

        <!-----------------/title------------------>

        <!-----------------content------------------>
        <div class="row">
            <div id="_Title" class="col-md-12 col-sm-12 col-xs-12">
                <div class="x_panel Div_Shadow">

                    <div class="x_content">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="x_panel">
                                <table id="TB" class="table table-bordered" border="1" cellspacing="0" style="width: 100%">
                                    <tr id="tr_row">
                                        <%=th%>
                                    </tr>
                                    <tbody>
                                        <%=tr%>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
    <!-- /Modal -->

    <!-- /Modal -->
    <!-- jQuery -->
    <script src="../../assets/vendors/jquery/dist/jquery.min.js"></script>
    <!-- Bootstrap -->
    <script src="../../assets/vendors/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- bootstrap-progressbar -->
    <script src="../../assets/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"></script>
    <!-- iCheck -->
    <script src="../../assets/vendors/iCheck/icheck.min.js"></script>
    <!-- bootstrap-wysiwyg -->
    <script src="../../assets/vendors/bootstrap-wysiwyg/js/bootstrap-wysiwyg.min.js"></script>
    <script src="../../assets/vendors/jquery.hotkeys/jquery.hotkeys.js"></script>
    <script src="../../assets/vendors/google-code-prettify/src/prettify.js"></script>
    <!-- jQuery Tags Input -->
    <script src="../../assets/vendors/jquery.tagsinput/src/jquery.tagsinput.js"></script>
    <!-- Switchery -->
    <script src="../../assets/vendors/switchery/dist/switchery.min.js"></script>
    <!-- Select2 -->
    <script src="../../assets/vendors/select2/dist/js/select2.full.min.js"></script>
    <!-- Autosize -->
    <script src="../../assets/vendors/autosize/dist/autosize.min.js"></script>
    <!-- jQuery autocomplete -->
    <script src="../../assets/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"></script>
    <!-- starrr -->
    <script src="../../assets/vendors/starrr/dist/starrr.js"></script>
    <!-- Custom Theme Scripts -->
    <script src="../../assets/build/js/custom.min.js"></script>
    <!-- FloatingActionButton -->
    <script src="../../assets/vendors/FloatingActionButton/js/index.js"></script>
    <!-- canvasjs -->
    <script src="../../assets/vendors/canvas_js/canvasjs.min.js"></script>
    <!-- Datatables -->
    <script src="../../assets/vendors/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="../../assets/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/dataTables.buttons.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.flash.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.html5.min.js"></script>
    <script src="../../assets/vendors/datatables.net-buttons/js/buttons.print.min.js"></script>
    <script src="../../assets/vendors/datatables.net-responsive/js/dataTables.responsive.min.js"></script>
    <script src="../../assets/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js"></script>
    <script src="../../assets/vendors/datatables.net-scroller/js/dataTables.scroller.min.js"></script>
    <script src="../../assets/vendors/datatables.net-colReorder/dataTables.colReorder.min.js"></script>
    <script src="../../assets/vendors/jszip/dist/jszip.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/pdfmake.min.js"></script>
    <script src="../../assets/vendors/pdfmake/build/vfs_fonts.js"></script>
    <script src="../../assets/vendors/time/loading.js"></script>

</asp:Content>
