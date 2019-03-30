<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="QuickBooksIntegration.Index" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <% if (HttpContext.Current.Session["accessToken"] != null && HttpContext.Current.Session["callMadeBy"] != null)
        {
            if (HttpContext.Current.Session["callMadeBy"].ToString() != "OpenId")
            {

                Response.Write("<script> window.opener.location.reload();window.close(); </script>");

            }
        }
    %>
    <div id="connect" runat="server" visible="false">
        <br />
        <!-- Connect To QuickBooks Button -->
        <b>Connect To QuickBooks</b><br />
        <asp:ImageButton ID="btnC2QB" runat="server" AlternateText="Connect to Quickbooks"
            ImageAlign="left"
            ImageUrl="Images/C2QB_white_btn_lg_default.png"
            OnClick="ImgC2QB_Click" Height="40px" Width="200px" />
        <br />
        <br />
        <br />
    </div>
    <div id="revoke" runat="server" visible="false">
        <div class="row">
            <br />
            <h4>QuickBook Actions</h4>
            <hr />
            <asp:Label runat="server" ID="lblConnected" Visible="false">"Your application is connected!"</asp:Label>
            <table style="width: 100%;">
                <tr>
                    <td>
                        <asp:Button ID="btnQBOAPICall" CssClass="btn btn-info" runat="server" Text="Call QBO API" OnClick="ImgQBOAPICall_Click" />
                        <asp:Label runat="server" ID="lblQBOCall" Visible="false"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="btnRevoke" runat="server" CssClass="btn btn-info" Text="Revoke Tokens" OnClick="ImgRevoke_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <hr />
        <div class="form-group">
            <h3>
                <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label></h3>
            <div class="row">
                <div class="col-md-6">
                    <h4>Module</h4>
                    <asp:DropDownList ID="ddlModule" class="form-control" runat="server" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="0">Select a Module</asp:ListItem>
                        <asp:ListItem Value="Account">Account</asp:ListItem>
                        <asp:ListItem Value="Customer">Customer</asp:ListItem>
                        <asp:ListItem Value="Payment">Payment</asp:ListItem>
                        <asp:ListItem Value="Vendor">Vendor</asp:ListItem>
                        <asp:ListItem Value="Bill">Bill</asp:ListItem>
                        <asp:ListItem Value="BillPayment">BillPayment</asp:ListItem>
                        <asp:ListItem Value="Invoice">Invoice</asp:ListItem>
                        <asp:ListItem Value="Purchase">Purchase</asp:ListItem>
                        <asp:ListItem Value="SalesReceipt">SalesReceipt</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <br />
            <div id="divCustomer" runat="server" visible="false">
                <h4>Customer</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlCustomerOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlCustomerOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Customer Option</asp:ListItem>
                            <asp:ListItem Value="CreateCustomer">Create Customer</asp:ListItem>
                            <asp:ListItem Value="GetCustomerById">Get Customer By ID</asp:ListItem>
                            <asp:ListItem Value="CustomerUpdate">Customer Update</asp:ListItem>
                            <asp:ListItem Value="CustomerDelete">Customer Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllCustomers">Read All Customers</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateCustomer" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtCCBillingAddressLine1" class="form-control" runat="server" placeholder="Billing Address Line 1">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtCCBillingAddressCity" class="form-control" runat="server" placeholder="Billing Address City">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtCCBillingAddressCountry" class="form-control" runat="server" placeholder="Billing Address Country">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtCCNotes" class="form-control" runat="server" placeholder="Notes">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtCCName" class="form-control" runat="server" placeholder="Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateCustomer" class="btn btn-primary" runat="server" Text="Create Customer" OnClick="btnCreateCustomer_Click" />
                        </div>
                    </div>
                </div>
                <div id="divCustomerReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtCustomerId" class="form-control" runat="server" Placeholder="Customer Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetCustomerById" class="btn btn-primary" runat="server" Text="Get Customer By Id" OnClick="btnGetCustomerById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateCustomer" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCCustomerId" class="form-control" runat="server" placeholder="Customer Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCCustomerName" class="form-control" runat="server" placeholder="Customer Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateCustomer" class="btn btn-primary" runat="server" Text="Update Customer" OnClick="btnUpdateCustomer_Click" />
                        </div>
                    </div>
                </div>
                <div id="divCustomerDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtCDId" class="form-control" runat="server" Placeholder="Customer Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnCustomerDelete" class="btn btn-primary" runat="server" Text="Delete Customer By Id" OnClick="btnCustomerDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divCustomers" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetCustomers" class="btn btn-primary" runat="server" Text="Get Customers" OnClick="btnGetCustomers_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwCustomers" runat="server" Caption="Customer" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divAccount" runat="server" visible="false">
                <h4>Customer</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlAccountOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlAccountOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Account Option</asp:ListItem>
                            <asp:ListItem Value="CreateAccount">Create Account</asp:ListItem>
                            <asp:ListItem Value="GetAccountById">Get Account By ID</asp:ListItem>
                            <asp:ListItem Value="AccountUpdate">Account Update</asp:ListItem>
                            <asp:ListItem Value="ReadAllAccounts">Read All Accounts</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateAccount" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-2">
                            <asp:DropDownList ID="ddlAccountType" class="form-control" runat="server">
                                <asp:ListItem Value="Other Asset">Other Asset</asp:ListItem>
                                <asp:ListItem Value="Non-Posting">Non-Posting</asp:ListItem>
                                <asp:ListItem Value="Fixed Asset">Fixed Asset</asp:ListItem>
                                <asp:ListItem Value="Equity">Equity</asp:ListItem>
                                <asp:ListItem Value="Accounts Payable">Accounts Payable</asp:ListItem>
                                <asp:ListItem Value="Income">Income</asp:ListItem>
                                <asp:ListItem Value="Cost of Goods Sold">Cost of Goods Sold</asp:ListItem>
                                <asp:ListItem Value="Accounts Receivable">Accounts Receivable</asp:ListItem>
                                <asp:ListItem Value="Credit Card">Credit Card</asp:ListItem>
                                <asp:ListItem Value="Expense">Expense</asp:ListItem>
                                <asp:ListItem Value="Other Current Asset">Other Current Asset</asp:ListItem>
                                <asp:ListItem Value="Bank">Bank</asp:ListItem>
                                <asp:ListItem Value="Long Term Liability">Long Term Liability</asp:ListItem>
                                <asp:ListItem Value="Other Expense">Other Expense</asp:ListItem>
                                <asp:ListItem Value="Other Current Liability">Other Current Liability</asp:ListItem>
                                <asp:ListItem Value="Other Income">Other Income</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtAccountName" class="form-control" runat="server" placeholder="Account Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateAccount" class="btn btn-primary" runat="server" Text="Create Account" OnClick="btnCreateAccount_Click" />
                        </div>
                    </div>
                </div>
                <div id="divAccountReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtAccountId" class="form-control" runat="server" Placeholder="Account Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetAccountById" class="btn btn-primary" runat="server" Text="Get Account By Id" OnClick="btnGetAccountById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateAccount" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCAccountId" class="form-control" runat="server" placeholder="Account Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCAccountName" class="form-control" runat="server" placeholder="Account Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateAccount" class="btn btn-primary" runat="server" Text="Update Account" OnClick="btnUpdateAccount_Click" />
                        </div>
                    </div>
                </div>
                <div id="divAccounts" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetAccounts" class="btn btn-primary" runat="server" Text="Get Accounts" OnClick="btnGetAccounts_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwAccounts" runat="server" Caption="Account" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divVendor" runat="server" visible="false">
                <h4>Vendor</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlVendorOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlVendorOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Vendor Option</asp:ListItem>
                            <asp:ListItem Value="CreateVendor">Create Vendor</asp:ListItem>
                            <asp:ListItem Value="GetVendorById">Get Vendor By ID</asp:ListItem>
                            <asp:ListItem Value="VendorUpdate">Vendor Update</asp:ListItem>
                            <asp:ListItem Value="VendorDelete">Vendor Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllVendors">Read All Vendors</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateVendor" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtVendorBillingAddressLine1" class="form-control" runat="server" placeholder="Billing Address Line 1">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtVendorBillingAddressCity" class="form-control" runat="server" placeholder="Billing Address City">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtVendorBillingAddressCountry" class="form-control" runat="server" placeholder="Billing Address Country">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:TextBox ID="txtVendorName" class="form-control" runat="server" placeholder="Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateVendor" class="btn btn-primary" runat="server" Text="Create Vendor" OnClick="btnCreateVendor_Click" />
                        </div>
                    </div>
                </div>
                <div id="divVendorReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVendorId" class="form-control" runat="server" Placeholder="Vendor Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetVendorById" class="btn btn-primary" runat="server" Text="Get Vendor By Id" OnClick="btnGetVendorById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateVendor" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCVendorId" class="form-control" runat="server" placeholder="Vendor Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCVendorName" class="form-control" runat="server" placeholder="Vendor Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateVendor" class="btn btn-primary" runat="server" Text="Update Vendor" OnClick="btnUpdateVendor_Click" />
                        </div>
                    </div>
                </div>
                <div id="divVendorDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDVendorId" class="form-control" runat="server" Placeholder="Vendor Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnVendorDelete" class="btn btn-primary" runat="server" Text="Delete Vendor By Id" OnClick="btnVendorDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divVendors" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetVendors" class="btn btn-primary" runat="server" Text="Get Vendors" OnClick="btnGetVendors_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwVendors" runat="server" Caption="Vendor" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divPayment" runat="server" visible="false">
                <h4>Payment</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlPaymentOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlPaymentOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Payment Option</asp:ListItem>
                            <asp:ListItem Value="CreatePayment">Create Payment</asp:ListItem>
                            <asp:ListItem Value="GetPaymentById">Get Payment By ID</asp:ListItem>
                            <asp:ListItem Value="PaymentUpdate">Payment Update</asp:ListItem>
                            <asp:ListItem Value="PaymentDelete">Payment Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllPayments">Read All Payments</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreatePayment" runat="server" visible="false">
                    <div class="row">


                        <div class="col-md-2">
                            <asp:Button ID="btnCreatePayment" class="btn btn-primary" runat="server" Text="Create Payment" OnClick="btnCreatePayment_Click" />
                        </div>
                    </div>
                </div>
                <div id="divPaymentReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtPaymentId" class="form-control" runat="server" Placeholder="Payment Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetPaymentById" class="btn btn-primary" runat="server" Text="Get Payment By Id" OnClick="btnGetPaymentById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdatePayment" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCPaymentId" class="form-control" runat="server" placeholder="Payment Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCPaymentName" class="form-control" runat="server" placeholder="Payment Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdatePayment" class="btn btn-primary" runat="server" Text="Update Payment" OnClick="btnUpdatePayment_Click" />
                        </div>
                    </div>
                </div>
                <div id="divPaymentDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDPaymentId" class="form-control" runat="server" Placeholder="Payment Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnPaymentDelete" class="btn btn-primary" runat="server" Text="Delete Payment By Id" OnClick="btnPaymentDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divPayments" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetPayments" class="btn btn-primary" runat="server" Text="Get Payments" OnClick="btnGetPayments_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwPayments" runat="server" Caption="Payment" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divBill" runat="server" visible="false">
                <h4>Bill</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlBillOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlBillOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Bill Option</asp:ListItem>
                            <asp:ListItem Value="CreateBill">Create Bill</asp:ListItem>
                            <asp:ListItem Value="GetBillById">Get Bill By ID</asp:ListItem>
                            <asp:ListItem Value="BillUpdate">Bill Update</asp:ListItem>
                            <asp:ListItem Value="BillDelete">Bill Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllBills">Read All Bills</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateBill" runat="server" visible="false">
                    <p>Bill is hardcoded to particular vendor</p>
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtBillAmount" class="form-control" runat="server" placeholder="Bill Amount">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateBill" class="btn btn-primary" runat="server" Text="Create Bill" OnClick="btnCreateBill_Click" />
                        </div>
                    </div>
                </div>
                <div id="divBillReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtBillId" class="form-control" runat="server" Placeholder="Bill Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetBillById" class="btn btn-primary" runat="server" Text="Get Bill By Id" OnClick="btnGetBillById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateBill" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCBillId" class="form-control" runat="server" placeholder="Bill Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCBillName" class="form-control" runat="server" placeholder="Bill Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateBill" class="btn btn-primary" runat="server" Text="Update Bill" OnClick="btnUpdateBill_Click" />
                        </div>
                    </div>
                </div>
                <div id="divBillDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDBillId" class="form-control" runat="server" Placeholder="Bill Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnBillDelete" class="btn btn-primary" runat="server" Text="Delete Bill By Id" OnClick="btnBillDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divBills" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetBills" class="btn btn-primary" runat="server" Text="Get Bills" OnClick="btnGetBills_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwBills" runat="server" Caption="Bill" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divBillPayment" runat="server" visible="false">
                <h4>BillPayment</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlBillPaymentOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlBillPaymentOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a BillPayment Option</asp:ListItem>
                            <asp:ListItem Value="CreateBillPayment">Create BillPayment</asp:ListItem>
                            <asp:ListItem Value="GetBillPaymentById">Get BillPayment By ID</asp:ListItem>
                            <asp:ListItem Value="BillPaymentUpdate">BillPayment Update</asp:ListItem>
                            <asp:ListItem Value="BillPaymentDelete">BillPayment Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllBillPayments">Read All BillPayments</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateBillPayment" runat="server" visible="false">
                    <p>BillPayment is hardcoded to particular vendor</p>
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtBillPaymentAmount" class="form-control" runat="server" placeholder="BillPayment Amount">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateBillPayment" class="btn btn-primary" runat="server" Text="Create BillPayment" OnClick="btnCreateBillPayment_Click" />
                        </div>
                    </div>
                </div>
                <div id="divBillPaymentReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtBillPaymentId" class="form-control" runat="server" Placeholder="BillPayment Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetBillPaymentById" class="btn btn-primary" runat="server" Text="Get BillPayment By Id" OnClick="btnGetBillPaymentById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateBillPayment" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCBillPaymentId" class="form-control" runat="server" placeholder="BillPayment Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCBillPaymentName" class="form-control" runat="server" placeholder="BillPayment Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateBillPayment" class="btn btn-primary" runat="server" Text="Update BillPayment" OnClick="btnUpdateBillPayment_Click" />
                        </div>
                    </div>
                </div>
                <div id="divBillPaymentDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDBillPaymentId" class="form-control" runat="server" Placeholder="BillPayment Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnBillPaymentDelete" class="btn btn-primary" runat="server" Text="Delete BillPayment By Id" OnClick="btnBillPaymentDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divBillPayments" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetBillPayments" class="btn btn-primary" runat="server" Text="Get BillPayments" OnClick="btnGetBillPayments_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwBillPayments" runat="server" Caption="BillPayment" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divInvoice" runat="server" visible="false">
                <h4>Invoice</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlInvoiceOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlInvoiceOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Invoice Option</asp:ListItem>
                            <asp:ListItem Value="CreateInvoice">Create Invoice</asp:ListItem>
                            <asp:ListItem Value="GetInvoiceById">Get Invoice By ID</asp:ListItem>
                            <asp:ListItem Value="InvoiceUpdate">Invoice Update</asp:ListItem>
                            <asp:ListItem Value="InvoiceDelete">Invoice Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllInvoices">Read All Invoices</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateInvoice" runat="server" visible="false">
                    <p>Invoice is hardcoded to particular vendor</p>
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtInvoiceAmount" class="form-control" runat="server" placeholder="Invoice Amount">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateInvoice" class="btn btn-primary" runat="server" Text="Create Invoice" OnClick="btnCreateInvoice_Click" />
                        </div>
                    </div>
                </div>
                <div id="divInvoiceReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtInvoiceId" class="form-control" runat="server" Placeholder="Invoice Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetInvoiceById" class="btn btn-primary" runat="server" Text="Get Invoice By Id" OnClick="btnGetInvoiceById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateInvoice" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCInvoiceId" class="form-control" runat="server" placeholder="Invoice Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCInvoiceName" class="form-control" runat="server" placeholder="Invoice Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateInvoice" class="btn btn-primary" runat="server" Text="Update Invoice" OnClick="btnUpdateInvoice_Click" />
                        </div>
                    </div>
                </div>
                <div id="divInvoiceDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDInvoiceId" class="form-control" runat="server" Placeholder="Invoice Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnInvoiceDelete" class="btn btn-primary" runat="server" Text="Delete Invoice By Id" OnClick="btnInvoiceDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divInvoices" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetInvoices" class="btn btn-primary" runat="server" Text="Get Invoices" OnClick="btnGetInvoices_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwInvoices" runat="server" Caption="Invoice" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divPurchase" runat="server" visible="false">
                <h4>Purchase</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlPurchaseOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlPurchaseOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a Purchase Option</asp:ListItem>
                            <asp:ListItem Value="CreatePurchase">Create Purchase</asp:ListItem>
                            <asp:ListItem Value="GetPurchaseById">Get Purchase By ID</asp:ListItem>
                            <asp:ListItem Value="PurchaseUpdate">Purchase Update</asp:ListItem>
                            <asp:ListItem Value="PurchaseDelete">Purchase Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllPurchases">Read All Purchases</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreatePurchase" runat="server" visible="false">
                    <p>Purchase is hardcoded to particular vendor</p>
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtPurchaseAmount" class="form-control" runat="server" placeholder="Purchase Amount">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreatePurchase" class="btn btn-primary" runat="server" Text="Create Purchase" OnClick="btnCreatePurchase_Click" />
                        </div>
                    </div>
                </div>
                <div id="divPurchaseReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtPurchaseId" class="form-control" runat="server" Placeholder="Purchase Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetPurchaseById" class="btn btn-primary" runat="server" Text="Get Purchase By Id" OnClick="btnGetPurchaseById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdatePurchase" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCPurchaseId" class="form-control" runat="server" placeholder="Purchase Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCPurchaseName" class="form-control" runat="server" placeholder="Purchase Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdatePurchase" class="btn btn-primary" runat="server" Text="Update Purchase" OnClick="btnUpdatePurchase_Click" />
                        </div>
                    </div>
                </div>
                <div id="divPurchaseDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDPurchaseId" class="form-control" runat="server" Placeholder="Purchase Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnPurchaseDelete" class="btn btn-primary" runat="server" Text="Delete Purchase By Id" OnClick="btnPurchaseDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divPurchases" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetPurchases" class="btn btn-primary" runat="server" Text="Get Purchases" OnClick="btnGetPurchases_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwPurchases" runat="server" Caption="Purchase" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divSalesReceipt" runat="server" visible="false">
                <h4>SalesReceipt</h4>
                <div class="row">
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlSalesReceiptOptions" class="form-control" runat="server" OnSelectedIndexChanged="ddlSalesReceiptOptions_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="0">Select a SalesReceipt Option</asp:ListItem>
                            <asp:ListItem Value="CreateSalesReceipt">Create SalesReceipt</asp:ListItem>
                            <asp:ListItem Value="GetSalesReceiptById">Get SalesReceipt By ID</asp:ListItem>
                            <asp:ListItem Value="SalesReceiptUpdate">SalesReceipt Update</asp:ListItem>
                            <asp:ListItem Value="SalesReceiptDelete">SalesReceipt Delete</asp:ListItem>
                            <asp:ListItem Value="ReadAllSalesReceipts">Read All SalesReceipts</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <br />
                <div id="divCreateSalesReceipt" runat="server" visible="false">
                    <p>SalesReceipt is hardcoded to particular vendor</p>
                    <div class="row">
                        <div class="col-md-2">
                            <asp:TextBox ID="txtSalesReceiptAmount" class="form-control" runat="server" placeholder="SalesReceipt Amount">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnCreateSalesReceipt" class="btn btn-primary" runat="server" Text="Create SalesReceipt" OnClick="btnCreateSalesReceipt_Click" />
                        </div>
                    </div>
                </div>
                <div id="divSalesReceiptReadById" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtSalesReceiptId" class="form-control" runat="server" Placeholder="SalesReceipt Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnGetSalesReceiptById" class="btn btn-primary" runat="server" Text="Get SalesReceipt By Id" OnClick="btnGetSalesReceiptById_Click" />
                        </div>
                    </div>
                </div>
                <div id="divUpdateSalesReceipt" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCSalesReceiptId" class="form-control" runat="server" placeholder="SalesReceipt Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtUCSalesReceiptName" class="form-control" runat="server" placeholder="SalesReceipt Name">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnUpdateSalesReceipt" class="btn btn-primary" runat="server" Text="Update SalesReceipt" OnClick="btnUpdateSalesReceipt_Click" />
                        </div>
                    </div>
                </div>
                <div id="divSalesReceiptDelete" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtVDSalesReceiptId" class="form-control" runat="server" Placeholder="SalesReceipt Id">
                            </asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:Button ID="btnSalesReceiptDelete" class="btn btn-primary" runat="server" Text="Delete SalesReceipt By Id" OnClick="btnSalesReceiptDelete_Click" />
                        </div>
                    </div>
                </div>
                <div id="divSalesReceipts" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Button ID="btnGetSalesReceipts" class="btn btn-primary" runat="server" Text="Get SalesReceipts" OnClick="btnGetSalesReceipts_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvwSalesReceipts" runat="server" Caption="SalesReceipt" CaptionAlign="Top" AutoGenerateColumns="true" Width="90%" HeaderStyle-BackColor="#3AC0F2"
                                HeaderStyle-ForeColor="White" RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
                                RowStyle-ForeColor="#3A3A3A">
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
