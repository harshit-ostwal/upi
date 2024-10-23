using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS_SOFTWARE_UPI_PAYMENT
{
    public partial class FRM_PAYMENT : Form
    {
        string path = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source =" + Application.StartupPath + "/DATABASE/Main_db.accdb;Jet OLEDB:Database Password = SS9975";
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source =" + Application.StartupPath + "/DATABASE/Main_db.accdb;Jet OLEDB:Database Password = SS9975");
        OleDbCommand cmd = new OleDbCommand();
        int i = 0;

        public FRM_PAYMENT()
        {
            InitializeComponent();
        }

        private void FRM_PAYMENT_Load(object sender, EventArgs e)
        {
            BankDetails();
            AutoUPIData();
            ManualUPIData();
            pnlautoupi.Visible = false;
            pnlmanualupi.Visible = false;
            pnlbank.Visible = false;
        }

        private void btnqr_Click(object sender, EventArgs e)
        {
            PictureBox picture = sender as PictureBox;
            openFileDialog1.Title = "SELECT QR CODE IMAGE FROM YOUR DEVICE";
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = false;
            openFileDialog1.AddExtension = true;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ValidateNames = true;
            openFileDialog1.Filter = "Image Files(*.jpg,*.jpeg,*.png,*.gif,*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.ShowDialog();
            try
            {
                picture.Image = Image.FromFile(openFileDialog1.FileName);
                MessageBox.Show("QR CODE SELECTED!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("PLEASE SELECT YOUR QR CODE IMAGE???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }



        }

        private void btnmanualupi_Click(object sender, EventArgs e)
        {
            if (pnlmanualupi.Visible == false)
            {
                pnlmanualupi.Visible = true;
                pnlautoupi.Visible = false;
                pnlbank.Visible = false;
                pnlmain.Visible = false;
            }
            else
            {
                pnlmanualupi.Visible = false; 
                pnlautoupi.Visible = false;
                pnlbank.Visible = false;
                pnlmain.Visible = true;
            }
        }

        private void btnbackmanualupi_Click(object sender, EventArgs e)
        {
            pnlautoupi.Visible = false;
            pnlmanualupi.Visible = false;
            pnlbank.Visible = false;
            pnlmain.Visible = true;
        }

        private void btnpicclear_Click(object sender, EventArgs e)
        {
            btnqr.Image = null;
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            Clearall();
        }

        private void Clearall()
        {
            txtupiname.Text = "";
            btnqr.Image = null;
            txtupiname.Focus();
        }

        private void ManualUPIData()
        {
            string str = "select ID,f_upi_name,f_qr_code from Manual_UPI_db";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OleDbDataAdapter ad = new OleDbDataAdapter(str, path);
            ad.Fill(ds);
            dgw_manual_upi.DataSource = ds.Tables[0];
            dgw_manual_upi.Columns[0].HeaderText = "ID";
            dgw_manual_upi.Columns[1].HeaderText = "UPI NAME";
            dgw_manual_upi.Columns[2].HeaderText = "QR CODE";
            dgw_manual_upi.Columns[0].Visible = false;
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("DO YOU WANT TO SAVE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (btnqr.Image != null)
                    {
                        string location = "" + Application.StartupPath + "\\PHOTOS";
                        string photo = Path.Combine(location, txtupiname.Text + ".png");
                        con = new OleDbConnection(path);
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandText = "Insert into Manual_UPI_db (f_upi_name,f_qr_code) Values ('" + txtupiname.Text + "','" + photo + "')";
                        Image image = btnqr.Image;
                        cmd.ExecuteNonQuery();
                        image.Save(photo);
                        con.Close();
                        ManualUPIData();
                        string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                        string SP = Application.StartupPath + "\\BACKUP\\";
                        string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                        File.Copy(ThisDB, Destitnation);
                        MessageBox.Show("ADDED SUCCESSFULLY!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Clearall();
                    }
                    else
                    {
                        MessageBox.Show("PLEASE SELECT YOUR QR CODE IMAGE???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO SAVE DATA???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("DO YOU WANT TO DELETE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con = new OleDbConnection(path);
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM Manual_UPI_db  Where ID=" + dgw_manual_upi.SelectedRows[i].Cells[0].Value.ToString() + "";
                    cmd.ExecuteNonQuery();
                    string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                    string SP = Application.StartupPath + "\\BACKUP\\";
                    string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                    string location = Application.StartupPath + "\\PHOTOS\\" + txtupiname.Text + ".png";
                    File.Delete(location);
                    File.Copy(ThisDB, Destitnation);
                    con.Close();
                    ManualUPIData();
                    MessageBox.Show("DELETED SUCCESSFULLY", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clearall();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO DELETE DATA???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_details_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                txtupiname.Text = dgw_manual_upi.Rows[i].Cells[1].Value.ToString();
                btnqr.ImageLocation = dgw_manual_upi.Rows[i].Cells[2].Value.ToString();
            }
            catch (Exception)
            {

            }
        }

        private void dgw_details_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                txtupiname.Text = dgw_manual_upi.Rows[i].Cells[1].Value.ToString();
                btnqr.ImageLocation = dgw_manual_upi.Rows[i].Cells[2].Value.ToString();
            }
            catch (Exception)
            {

            }
        }

        private void ClearallAuto()
        {
            txtautoupiid.Text = "";
            txtautopayeename.Text = "";
            txtautoupiid.Focus();
        }

        private void btnautonew_Click(object sender, EventArgs e)
        {
            ClearallAuto();
        }

        private void AutoUPIData()
        {
            string str = "select ID,f_upi_id,f_payee_name from Auto_UPI_db";
            DataSet ds = new DataSet();
            OleDbDataAdapter ad = new OleDbDataAdapter(str, con);
            ad.Fill(ds);
            dgw_auto_upi.DataSource = ds.Tables[0];
            dgw_auto_upi.Columns[0].HeaderText = "ID";
            dgw_auto_upi.Columns[1].HeaderText = "UPI ID";
            dgw_auto_upi.Columns[2].HeaderText = "PAYEE NAME";
            dgw_auto_upi.Columns[0].Visible = false;
            ClearallAuto();
        }


        private void btnautosave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("DO YOU WANT TO SAVE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source =" + Application.StartupPath + "/DATABASE/Main_db.accdb;Jet OLEDB:Database Password = SS9975";
                    using (OleDbConnection con1 = new OleDbConnection(strcon))
                    {
                        con1.Open();
                        string Data = "select * from Auto_UPI_db where f_upi_id='" + txtautoupiid.Text + "'";
                        using (OleDbCommand cmd = new OleDbCommand(Data, con1))
                        {
                            int Count = Convert.ToInt32(cmd.ExecuteScalar());
                            if (Count > 0)
                            {
                                MessageBox.Show("CUSTOMER NAME IS ALREADY EXISTED???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (MessageBox.Show("DO YOU WANT TO SAVE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    if (txtautoupiid.Text != "" && txtautopayeename.Text != "")
                                    {
                                        con = new OleDbConnection(path);
                                        con.Open();
                                        cmd.Connection = con;
                                        cmd.CommandText = "Insert into Auto_UPI_db (f_upi_id,f_payee_name) Values ('" + txtautoupiid.Text + "','" + txtautopayeename.Text + "')";
                                        cmd.ExecuteNonQuery();
                                        con.Close();
                                        AutoUPIData();
                                        string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                                        string SP = Application.StartupPath + "\\BACKUP\\";
                                        string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                                        File.Copy(ThisDB, Destitnation);
                                        MessageBox.Show("ADDED SUCCESSFULLY!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        ClearallAuto();
                                    }
                                    else
                                    {
                                        MessageBox.Show("PLEASE FILL THE UPI DETAILS???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                        AutoUPIData();
                                        ClearallAuto();
                                    }
                                }
                                else
                                {
                                    ClearallAuto();
                                    AutoUPIData();
                                }
                            }
                            else
                            {
                                if (txtautoupiid.Text != "" && txtautopayeename.Text != "")
                                {
                                    con = new OleDbConnection(path);
                                    con.Open();
                                    cmd.Connection = con;
                                    cmd.CommandText = "Insert into Auto_UPI_db (f_upi_id,f_payee_name) Values ('" + txtautoupiid.Text + "','" + txtautopayeename.Text + "')";
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                    AutoUPIData();
                                    string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                                    string SP = Application.StartupPath + "\\BACKUP\\";
                                    string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                                    File.Copy(ThisDB, Destitnation);
                                    MessageBox.Show("ADDED SUCCESSFULLY!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearallAuto();
                                }
                                else
                                {
                                    MessageBox.Show("PLEASE FILL THE UPI DETAILS???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                    AutoUPIData();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO SAVE???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnautodelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("DO YOU WANT TO DELETE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con = new OleDbConnection(path);
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM Auto_UPI_db  Where ID=" + dgw_auto_upi.SelectedRows[i].Cells[0].Value.ToString() + "";
                    cmd.ExecuteNonQuery();
                    string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                    string SP = Application.StartupPath + "\\BACKUP\\";
                    string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                    string location = Application.StartupPath + "\\PHOTOS\\" + txtupiname.Text + ".png";
                    File.Delete(location);
                    File.Copy(ThisDB, Destitnation);
                    con.Close();
                    AutoUPIData();
                    MessageBox.Show("DELETED SUCCESSFULLY", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearallAuto();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO DELETE DATA???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnautoedit_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("DO YOU WANT TO EDIT???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con = new OleDbConnection(path);
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE  Auto_UPI_db set f_upi_id='"+txtautoupiid.Text+"',f_payee_name='"+txtautopayeename.Text+"'  where ID=" + dgw_auto_upi.SelectedRows[i].Cells[0].Value.ToString() + "";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("EDITED SUCCESSFULLY", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearallAuto();
                    string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                    string SP = Application.StartupPath + "\\BACKUP\\";
                    string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                    File.Copy(ThisDB, Destitnation);
                    AutoUPIData();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO EDIT DATA???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_auto_upi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtautoupiid.Text = dgw_auto_upi.Rows[i].Cells[1].Value.ToString();
            txtautopayeename.Text = dgw_auto_upi.Rows[i].Cells[2].Value.ToString();
        }

        private void btnbackautoupi_Click(object sender, EventArgs e)
        {
            pnlautoupi.Visible = false;
            pnlmanualupi.Visible = false;
            pnlbank.Visible = false;
            pnlmain.Visible = true;
        }

        private void btnautoupi_Click(object sender, EventArgs e)
        {
            if (pnlautoupi.Visible == false)
            {
                pnlautoupi.Visible = true;
                pnlmanualupi.Visible = false;
                pnlmain.Visible = false;
                pnlbank.Visible = false;
            }
            else
            {
                pnlautoupi.Visible = false;
                pnlmanualupi.Visible = false;
                pnlbank.Visible = false;
                pnlmain.Visible = true;
            }
        }

        private void BankDetails()
        {
            string str = "Select ID,f_account_no,f_ifsc_code,f_payee_name from Bank_db";
            OleDbDataAdapter ad = new OleDbDataAdapter(str,con);
            DataSet ds = new DataSet();
            ad.Fill(ds);
            dgw_bank.DataSource = ds.Tables[0];
            dgw_bank.Columns[0].HeaderText = "ID";
            dgw_bank.Columns[0].Visible = false;
            dgw_bank.Columns[1].HeaderText = "ACCOUNT NO";
            dgw_bank.Columns[2].HeaderText = "IFSC CODE";
            dgw_bank.Columns[3].HeaderText = "PAYEE NAME";
        }

        private void ClearallBank()
        {
            txtaccountno.Text = "";
            txtifsccode.Text = "";
            txtbankpayeename.Text = "";
            txtaccountno.Focus();
        }

        private void btnbanksave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("DO YOU WANT TO SAVE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source =" + Application.StartupPath + "/DATABASE/Main_db.accdb;Jet OLEDB:Database Password = SS9975";
                    using (OleDbConnection con = new OleDbConnection(strcon))
                    {
                        con.Open();
                        string Data = "select * from Bank_db where f_account_no='" + txtaccountno.Text + "'";
                        string Data1 = "select * from Bank_db where f_ifsc_code='" + txtifsccode.Text + "'";
                        using (OleDbCommand cmd = new OleDbCommand(Data, con))
                        {
                            using (OleDbCommand cmd1 = new OleDbCommand(Data1, con))
                            {
                                int Count1 = Convert.ToInt32(cmd1.ExecuteScalar());
                                if (Count1 > 0)
                                {
                                    MessageBox.Show("CUSTOMER ID IS ALREADY EXISTED???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    ClearallBank();
                                    BankDetails();
                                }
                                else
                                {
                                    int Count = Convert.ToInt32(cmd.ExecuteScalar());
                                    if (Count > 0)
                                    {
                                        MessageBox.Show("CUSTOMER NAME IS ALREADY EXISTED???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        if (MessageBox.Show("DO YOU WANT TO SAVE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            if (txtaccountno.Text != "" && txtifsccode.Text != "")
                                            {
                                                cmd.CommandText = "Insert into Bank_db (f_account_no,f_ifsc_code,f_payee_name) Values ('" + txtaccountno.Text + "','" + txtifsccode.Text + "','" + txtbankpayeename.Text + "')";
                                                cmd.ExecuteNonQuery();
                                                MessageBox.Show("ADDED SUCCESSFULLY!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                Clearall();
                                                string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                                                string SP = Application.StartupPath + "\\BACKUP\\";
                                                string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                                                File.Copy(ThisDB, Destitnation);
                                                con.Close();
                                                ClearallBank();
                                                BankDetails();
                                            }
                                            else
                                            {
                                                MessageBox.Show("FILL ALL THE BOXES???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                Clearall();
                                                ClearallBank();
                                                BankDetails();
                                            }
                                        }
                                        else
                                        {
                                            Clearall();
                                            ClearallBank();
                                            BankDetails();
                                        }
                                    }
                                    else
                                    {
                                        if (txtaccountno.Text != "" && txtifsccode.Text != "")
                                        {
                                            cmd.CommandText = "Insert into Bank_db (f_account_no,f_ifsc_code,f_payee_name) Values ('" + txtaccountno.Text + "','" + txtifsccode.Text + "','" + txtbankpayeename.Text + "')";
                                            cmd.ExecuteNonQuery();
                                            MessageBox.Show("ADDED SUCCESSFULLY!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Clearall();
                                            string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                                            string SP = Application.StartupPath + "\\BACKUP\\";
                                            string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                                            File.Copy(ThisDB, Destitnation);
                                            con.Close();
                                            ClearallBank();
                                            BankDetails();
                                        }
                                        else
                                        {
                                            MessageBox.Show("FILL ALL THE BOXES???", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            Clearall();
                                            ClearallBank();
                                            BankDetails();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO SAVE???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnbanknew_Click(object sender, EventArgs e)
        {
            ClearallBank();
            BankDetails();
        }

        private void btnbankedit_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("DO YOU WANT TO EDIT???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con = new OleDbConnection(path);
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE  Bank_db set f_account_no='" + txtaccountno.Text + "',f_ifsc_code='" + txtifsccode.Text + "',f_payee_name='" + txtbankpayeename.Text + "'  where ID=" + dgw_bank.SelectedRows[i].Cells[0].Value.ToString() + "";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("EDITED SUCCESSFULLY", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearallBank();
                    string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                    string SP = Application.StartupPath + "\\BACKUP\\";
                    string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                    File.Copy(ThisDB, Destitnation);
                    BankDetails();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO EDIT DATA???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnbankdelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("DO YOU WANT TO DELETE???", "SS SOFTWARE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con = new OleDbConnection(path);
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM Bank_db  Where ID=" + dgw_bank.SelectedRows[i].Cells[0].Value.ToString() + "";
                    cmd.ExecuteNonQuery();
                    string ThisDB = Application.StartupPath + "\\DATABASE\\Main_db.accdb";
                    string SP = Application.StartupPath + "\\BACKUP\\";
                    string Destitnation = SP + "\\Main_db " + DateTime.Now.ToString(" dd-MM-yyyy hh-mm-ss") + ".bak";
                    string location = Application.StartupPath + "\\PHOTOS\\" + txtupiname.Text + ".png";
                    File.Delete(location);
                    File.Copy(ThisDB, Destitnation);
                    con.Close();
                    BankDetails();
                    MessageBox.Show("DELETED SUCCESSFULLY", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearallBank();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("UNABLE TO DELETE DATA???,PLS TRY AGAIN!!!", "SS SOFTWARE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_bank_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtaccountno.Text = dgw_bank.Rows[i].Cells[1].Value.ToString();
            txtifsccode.Text = dgw_bank.Rows[i].Cells[2].Value.ToString();
            txtbankpayeename.Text = dgw_bank.Rows[i].Cells[3].Value.ToString();
        }

        private void btnbackspacebank_Click(object sender, EventArgs e)
        {
            pnlautoupi.Visible = false;
            pnlmanualupi.Visible = false;
            pnlbank.Visible = false;
            pnlmain.Visible = true;
        }

        private void btnbank_Click(object sender, EventArgs e)
        {
            if(pnlbank.Visible==false)
            {
                pnlbank.Visible = true;
                pnlmain.Visible = false;
                pnlautoupi.Visible = false;
                pnlmanualupi.Visible = false;
            }
            else
            {
                pnlbank.Visible = false;
                pnlmain.Visible = true;
                pnlautoupi.Visible = false;
                pnlmanualupi.Visible = false;
            }
        }
    }
}
