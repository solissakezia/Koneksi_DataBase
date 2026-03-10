using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBKoneksi
{
    public partial class FrmFoto : Form
    {
        private MySqlConnection koneksi;
        private MySqlDataAdapter adapter;
        private MySqlCommand perintah;
        private DataSet ds = new DataSet();
        private string alamat, query;
        
        public FrmFoto()
        {
            alamat = "server=localhost; database=db_mahasiswa; username=root; password=;";
            koneksi = new MySqlConnection(alamat);
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text != "")
                {
                    query = string.Format("select * from tbl_pengguna where username = '{0}'", txtUsername.Text);
                    ds.Clear();
                    koneksi.Open();
                    perintah = new MySqlCommand(query, koneksi);
                    adapter = new MySqlDataAdapter(perintah);
                    perintah.ExecuteNonQuery();
                    adapter.Fill(ds);
                    koneksi.Close();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow kolom in ds.Tables[0].Rows)
                        {
                            txtID.Text = kolom["id_pengguna"].ToString();
                            txtPassword.Text = kolom["password"].ToString();
                            txtNama.Text = kolom["nama_pengguna"].ToString();
                            CBLevel.Text = kolom["level"].ToString();
                            string fileName = kolom["foto"].ToString();

                            string folderPath = Path.Combine(Application.StartupPath, @"C:\Users\HP\source\repos\DBKoneksi\DBKoneksi\Foto");
                            string filePath = Path.Combine(folderPath, fileName);

                            // Cek apakah file foto ada
                            if (File.Exists(filePath))
                            {
                                // Tampilkan gambar di PictureBox
                                pictureBox1.Image = Image.FromFile(filePath);
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                MessageBox.Show("File gambar tidak ditemukan.");
                            }



                        }
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = true;
                        btnDelete.Enabled = true;
                        btnSearch.Enabled = false;
                        btnClear.Enabled = true;
                        LblFoto.Visible = false;

                    }
                    else
                    {
                        MessageBox.Show("Data Tidak Ada !!");
                        FrmFoto_Load(null, null);
                    }

                }
                else
                {
                    MessageBox.Show("Data Yang Anda Pilih Tidak Ada !!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            LblFoto.Visible = false;
        }

      
            private void FrmFoto_Load(object sender, EventArgs e)
        {
            try
            {
                // Pastikan koneksi tertutup sebelum dibuka
                if (koneksi.State == ConnectionState.Open) { koneksi.Close(); }
                koneksi.Open();

                query = "select * from tbl_pengguna";
                perintah = new MySqlCommand(query, koneksi);
                adapter = new MySqlDataAdapter(perintah);

                ds.Clear();
                adapter.Fill(ds); // ExecuteNonQuery() dihapus karena tidak perlu untuk SELECT

                txtID.Clear();
                txtNama.Clear();
                txtPassword.Clear();
                txtUsername.Clear();
                txtID.Focus();
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                btnClear.Enabled = false;
                btnSave.Enabled = true;
                btnSearch.Enabled = true;
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Load Data: " + ex.Message);
            }
            finally
            {
                // Blok ini akan SELALU DILAKUKAN meskipun terjadi error
                koneksi.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != "" && txtNama.Text != "" && txtUsername.Text != "" && txtID.Text != "")
                {
                    // Tentukan folder tempat menyimpan gambar
                    string folderPath = Path.Combine(Application.StartupPath, "C:\\Users\\HP\\source\\repos\\DBKoneksi\\DBKoneksi\\Foto");

                    // Pastikan folder ada, jika tidak, buat folder
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Membuat nama unik untuk file gambar agar tidak tertimpa
                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                    // Simpan gambar dari PictureBox ke folder
                    pictureBox1.Image.Save(filePath);


                    query = string.Format("update tbl_pengguna set password = '{0}', nama_pengguna = '{1}', level = '{2}', foto = '{3}' where id_pengguna = '{4}'", txtPassword.Text, txtNama.Text, CBLevel.Text, fileName, txtID.Text);


                    koneksi.Open();
                    perintah = new MySqlCommand(query, koneksi);
                    adapter = new MySqlDataAdapter(perintah);
                    int res = perintah.ExecuteNonQuery();
                    koneksi.Close();
                    if (res == 1)
                    {
                        MessageBox.Show("Update Data Suksess ...");
                        FrmFoto_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Gagal Update Data . . . ");
                    }
                }
                else
                {
                    MessageBox.Show("Data Tidak lengkap !!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtID.Text != "")
                {
                    if (MessageBox.Show("Anda Yakin Menghapus Data Ini ??", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        query = string.Format("Delete from tbl_pengguna where id_pengguna = '{0}'", txtID.Text);
                        ds.Clear();
                        koneksi.Open();
                        perintah = new MySqlCommand(query, koneksi);
                        adapter = new MySqlDataAdapter(perintah);
                        int res = perintah.ExecuteNonQuery();
                        koneksi.Close();
                        if (res == 1)
                        {
                            MessageBox.Show("Delete Data Suksess ...");
                        }
                        else
                        {
                            MessageBox.Show("Gagal Delete data");
                        }
                    }
                    FrmFoto_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Data Yang Anda Pilih Tidak Ada !!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                FrmFoto_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text != "" && txtPassword.Text != "" && txtNama.Text != "" && pictureBox1.Image != null)
                {
                    // Perbaikan Path Folder (Lebih rapi)
                    string folderPath = @"C:\Users\HP\source\repos\DBKoneksi\DBKoneksi\Foto";

                    // Pastikan folder ada, jika tidak, buat folder
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Membuat nama unik untuk file gambar
                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                    // Simpan gambar dari PictureBox ke folder
                    pictureBox1.Image.Save(filePath);

                    // MENGGUNAKAN PARAMETER UNTUK MENCEGAH ERROR SYNTAX & FORMAT
                    query = "insert into tbl_pengguna values (@id, @user, @pass, @nama, @level, @file)";

                    if (koneksi.State == ConnectionState.Open) { koneksi.Close(); }
                    koneksi.Open();

                    perintah = new MySqlCommand(query, koneksi);

                    // Memasukkan isi textbox ke parameter query
                    perintah.Parameters.AddWithValue("@id", txtID.Text);
                    perintah.Parameters.AddWithValue("@user", txtUsername.Text);
                    perintah.Parameters.AddWithValue("@pass", txtPassword.Text);
                    perintah.Parameters.AddWithValue("@nama", txtNama.Text);
                    perintah.Parameters.AddWithValue("@level", CBLevel.Text);
                    perintah.Parameters.AddWithValue("@file", fileName);

                    // adapter = new MySqlDataAdapter(perintah); // <-- Dihapus karena INSERT tidak butuh DataAdapter

                    int res = perintah.ExecuteNonQuery();

                    if (res == 1)
                    {
                        MessageBox.Show("Insert Data Sukses ...");
                        FrmFoto_Load(null, null); // Refresh tampilan
                    }
                    else
                    {
                        MessageBox.Show("Gagal insert Data ...");
                    }
                }
                else
                {
                    MessageBox.Show("Data Tidak lengkap !! Pastikan semua kolom terisi dan foto sudah dipilih.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Simpan: " + ex.Message);
            }
            finally
            {
                // Pastikan koneksi selalu tertutup walaupun ada error saat simpan
                if (koneksi.State == ConnectionState.Open) { koneksi.Close(); }
            }
        }
    }
    
}
