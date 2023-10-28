using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace laba3
{
    public partial class Form1 : Form
    {
        private string connstring = String.Format("Server=localhost;Port=5432;User Id=postgres;Password=toor;Database=postgres;");
        private NpgsqlConnection conn;
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(connstring);
            Select();
        }

        private void Select()
        {
            try
            {
                conn.Open();
                sql = @"SELECT * FROM public.""Student"";";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();
                dgvData.DataSource = null;
                dgvData.DataSource = dt;
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                rowIndex = e.RowIndex;
                txtFirstname.Text = dgvData.Rows[e.RowIndex].Cells["st_firstname"].Value.ToString();
                txtMidname.Text = dgvData.Rows[e.RowIndex].Cells["st_midname"].Value.ToString();
                txtLastname.Text = dgvData.Rows[e.RowIndex].Cells["st_lastname"].Value.ToString();
                txtCurse.Text = dgvData.Rows[e.RowIndex].Cells["st_curse"].Value.ToString();
                txtMajor.Text = dgvData.Rows[e.RowIndex].Cells["st_major"].Value.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            rowIndex = -1;
            txtFirstname.Enabled = txtMidname.Enabled = txtLastname.Enabled = txtCurse.Enabled = txtMajor.Enabled = true;
            txtFirstname.Text = txtMidname.Text = txtLastname.Text = txtCurse.Text = txtMajor.Text = null;
            txtFirstname.Select();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(rowIndex < 0)
            {
                MessageBox.Show("Пожалуйста выберите студента для обновления");
                return;
            }
            txtFirstname.Enabled = txtMidname.Enabled = txtLastname.Enabled = txtCurse.Enabled = txtMajor.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int result = 0;
            if (rowIndex < 0)
            {
                MessageBox.Show("Пожалуйста выберите студента для удаления");
                return;
            }
            try
            {
                conn.Open();
                sql = $@"DELETE FROM public.""Student"" WHERE st_id = {int.Parse(dgvData.Rows[rowIndex].Cells["st_id"].Value.ToString())};";
                cmd = new  NpgsqlCommand(sql, conn);
                result = cmd.ExecuteNonQuery();
                conn.Close();
                if (result == 1)
                {
                    MessageBox.Show("Удаление выполнено");
                    rowIndex = -1;
                    Select();
                }
            }
            catch(Exception ex)
            {
                conn.Close();
                MessageBox.Show("Ошибка удаления. Error"+ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int result = 0;  
            if (rowIndex < 0)//insert
            {
                try
                {
                    conn.Open();
                    sql = $@"INSERT INTO public.""Student""(st_firstname, st_midname, st_lastname, st_curse, st_major) VALUES ('{txtFirstname.Text}', '{txtMidname.Text}', '{txtLastname.Text}', '{txtCurse.Text}', '{txtMajor}');";
                    
                    cmd = new NpgsqlCommand(sql, conn);
                    result = (int)cmd.ExecuteNonQuery();
                    conn.Close();
                    if(result == 1)
                    {
                        MessageBox.Show("Новый студент добавлен");
                        Select();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка добавления");
                    }
                    
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("Ошибка добавления. Error:" + ex.Message);
                }
            }
            else//update
            {
                try
                {
                    conn.Open();
                    sql = $@"UPDATE public.""Student"" SET st_firstname = '{txtFirstname.Text}', st_midname = '{txtMidname.Text}', st_lastname = '{txtLastname.Text}', st_curse = '{txtCurse.Text}', st_major = '{txtMajor.Text}'  WHERE st_id = {int.Parse(dgvData.Rows[rowIndex].Cells["st_id"].Value.ToString())}";
                    cmd = new NpgsqlCommand(sql, conn);
                    result = (int)cmd.ExecuteNonQuery();
                    conn.Close();
                    if(result == 1)
                    {
                        MessageBox.Show("Update successfully");
                        Select();
                    }
                    else
                    {
                        MessageBox.Show("Update fail");
                    }
                    Select();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("Updated fail. Error:" + ex.Message);
                }
            }
            result = 0;
            txtFirstname.Text = txtMidname.Text = txtLastname.Text = txtCurse.Text = txtMajor.Text = null;
            txtFirstname.Enabled = txtMidname.Enabled = txtLastname.Enabled = txtCurse.Enabled = txtMajor.Enabled = false;
        }
    }
}
