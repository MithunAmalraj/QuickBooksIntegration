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
        <asp:Label runat="server" ID="lblConnected" Visible="false">"Your application is connected!"</asp:Label>
        <table style="width: 100%;">
            <tr>
                <td>
                    <asp:ImageButton ID="btnQBOAPICall" runat="server" AlternateText="Call QBO API"
                        ImageAlign="left"
                        OnClick="ImgQBOAPICall_Click" CssClass="font-size:14px border: 1px solid grey; padding: 10px; color: red" Height="40px" Width="200px" />
                    <asp:Label runat="server" ID="lblQBOCall" Visible="false"></asp:Label>
                </td>
                <td>
                    <asp:ImageButton ID="btnRevoke" runat="server" AlternateText="Revoke Tokens"
                        ImageAlign="left"
                        OnClick="ImgRevoke_Click" CssClass="font-size:14px border: 1px solid grey; padding: 10px; color: red" Height="40px" Width="200px" />
                </td>
            </tr>
        </table>

        <h3>
            <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label></h3>
        <asp:DropDownList ID="ddlModule" runat="server" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Value="0">Select a Module</asp:ListItem>
            <asp:ListItem Value="Customer">Customer</asp:ListItem>
            <asp:ListItem Value="CustomerPayment">Customer Payments</asp:ListItem>
        </asp:DropDownList>
        <div id="divCustomer" runat="server" visible="false">
            <asp:DropDownList ID="ddlCustomerOptions" runat="server" OnSelectedIndexChanged="ddlCustomerOptions_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="0">Select a Customer Option</asp:ListItem>
                <asp:ListItem Value="CreateCustomer">Create Customer</asp:ListItem>
                <asp:ListItem Value="GetCustomerById">Get Customer By ID</asp:ListItem>
                <asp:ListItem Value="CustomerUpdate">Customer Update</asp:ListItem>
                <asp:ListItem Value="CustomerDelete">Customer Delete</asp:ListItem>
                <asp:ListItem Value="ReadAllCustomers">Read All Customers</asp:ListItem>
            </asp:DropDownList>
            <div id="divCreateCustomer" runat="server" visible="false">
                <asp:TextBox ID="txtCCBillingAddressLine1" runat="server" placeholder="Billing Address Line 1">
                </asp:TextBox>
                <asp:TextBox ID="txtCCBillingAddressCity" runat="server" placeholder="Billing Address City">
                </asp:TextBox>
                <asp:TextBox ID="txtCCBillingAddressCountry" runat="server" placeholder="Billing Address Country">
                </asp:TextBox>
                <asp:TextBox ID="txtCCNotes" runat="server" placeholder="Notes">
                </asp:TextBox>
                <asp:TextBox ID="txtCCName" runat="server" placeholder="Name">
                </asp:TextBox>
                <asp:Button ID="btnCreateCustomer" runat="server" Text="Create Customer" OnClick="btnCreateCustomer_Click" />
            </div>
            <div id="divCustomerReadById" runat="server" visible="false">
                <asp:TextBox ID="txtCustomerId" runat="server" Placeholder="Customer Id">
                </asp:TextBox>
                 <asp:Button ID="btnGetCustomerById" runat="server" Text="Get Customer By Id" OnClick="btnGetCustomerById_Click" />
            </div>
            <div id="divUpdateCustomer" runat="server" visible="false">
                <asp:TextBox ID="txtUCCustomerId" runat="server" placeholder="Customer Id">
                </asp:TextBox>
                <asp:TextBox ID="txtUCCustomerName" runat="server" placeholder="Customer Name">
                </asp:TextBox>
               <asp:Button ID="btnUpdateCustomer" runat="server" Text="Update Customer" OnClick="btnUpdateCustomer_Click" />
            </div>
            <div id="divCustomerDelete" runat="server" visible="false">
                <asp:TextBox ID="txtCDId" runat="server" Placeholder="Customer Id">
                </asp:TextBox>
                 <asp:Button ID="btnCustomerDelete" runat="server" Text="Delete Customer By Id" OnClick="btnCustomerDelete_Click" />
            </div>
              <div id="divCustomers" runat="server" visible="false">
            <asp:Button ID="Button1" runat="server" Text="Get Customers" OnClick="btnGetCustomers_Click" />
            <asp:GridView ID="gvwCustomers" runat="server" AutoGenerateColumns="true">
            </asp:GridView>
        </div>
        </div>
        <div id="divCustommerPayments" runat="server" visible="false">
            <asp:Button ID="btnGetCustomerPayments" runat="server" Text="Get Customer Payments" OnClick="btnGetPayments_Click" />
            <asp:GridView ID="gvwCustomerPayments" runat="server" AutoGenerateColumns="true">
            </asp:GridView>
        </div>
    </div>
</asp:Content>
