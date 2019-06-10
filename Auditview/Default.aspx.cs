using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    public Int64 cleanSize(object size)
    {
        int foo = size.ToString().Length;
        if (foo != 0)
            return (Convert.ToInt64(size) / 1048576);
        else
            return (0);
    }
}