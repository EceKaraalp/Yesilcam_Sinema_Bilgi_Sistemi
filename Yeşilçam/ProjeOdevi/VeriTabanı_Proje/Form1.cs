using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeriTabanı_Proje
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FilmleriGoster();
            OyunculariGoster();
            YonetmenGoster();
        }

        NpgsqlConnection baglanti = new NpgsqlConnection("server=localHost; port=5432;Database=SinemaBilgi;user ID=postgres;password=12345");

        private void FilmleriGoster()
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"filmTbl\"  ORDER BY \"FilmID\"", baglanti);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                dataGridView1.DataSource = dataTable;

                dataReader.Close();
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); //bağlantıyı kapat
                }
            }
        }

        private void OyunculariGoster()
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"oyuncuTbl\"  ORDER BY \"OyuncuID\"", baglanti);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                dataGridView2.DataSource = dataTable;

                dataReader.Close();
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); //bağlantıyı kapat
                }
            }
        }

        private void YonetmenGoster()
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"yonetmenTbl\"  ORDER BY \"YonetmenID\"", baglanti);

                NpgsqlDataReader dataReader = cmd.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                dataGridViewYonetmen.DataSource = dataTable;

                dataReader.Close();
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); //bağlantıyı kapat
                }
            }
        }

        private Image FromStream(MemoryStream ms)
        {
            throw new NotImplementedException();
        }


        private void buttonTemizle_Click(object sender, EventArgs e)
        {
            txtID.Clear();
            txtAd.Clear();
            txtYonetmen.Clear();
            txtRating.Clear();
            txtButce.Clear();
            txtYil.Clear();
            txtBoxGiseSayisi.Clear();
            txtResim.Clear();
            txtTur.Clear();
            pictureBox1.Image = null;
        }

        private void Ekle_Button_Click(object sender, EventArgs e)
        {
            if (baglanti.State != ConnectionState.Open)
            {
                baglanti.Open(); // bağlantıyı aç
            }

            try
            {
                byte[] resimByte;

                if (!string.IsNullOrEmpty(txtResim.Text) && System.IO.File.Exists(txtResim.Text))
                {
                    resimByte = System.IO.File.ReadAllBytes(txtResim.Text);
                }
                else
                {
                    resimByte = new byte[0];
                }

                // Yönetmen adını ve tür adını kullanarak ilgili ID'leri bul
                int yonetmenID = YonetmenIDBul(txtYonetmen.Text);
                int turID = TurIDBul(txtTur.Text);

                NpgsqlCommand komut1 = new NpgsqlCommand("INSERT INTO \"filmTbl\" (\"FilmID\",\"Ad\",\"Rating\",\"Butce\",\"YapimYili\",\"GiseSayısı\",\"Afis\",\"TurID\", \"YonetmenID\")" +
                    " VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9) RETURNING \"FilmID\"", baglanti);

                komut1.Parameters.AddWithValue("@p1", int.Parse(txtID.Text));
                komut1.Parameters.AddWithValue("@p2", txtAd.Text);
                komut1.Parameters.AddWithValue("@p3", Double.Parse(txtRating.Text));
                komut1.Parameters.AddWithValue("@p4", Double.Parse(txtButce.Text));
                komut1.Parameters.AddWithValue("@p5", int.Parse(txtYil.Text));
                komut1.Parameters.AddWithValue("@p6", int.Parse(txtBoxGiseSayisi.Text));
                komut1.Parameters.AddWithValue("@p7", resimByte);
                komut1.Parameters.AddWithValue("@p8", turID); // Tür ID'sini kullan
                komut1.Parameters.AddWithValue("@p9", yonetmenID); // Yönetmen ID'sini kullan

                komut1.ExecuteNonQuery();
                MessageBox.Show("Film ekleme işlemi başarılı");
                FilmleriGoster();
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); //bağlantıyı kapat
                }
            }
        }

        // Yönetmen adına göre YönetmenID'yi bulan metod
        private int YonetmenIDBul(string yonetmenAdi)
        {
            using (NpgsqlCommand yonetmenKomut = new NpgsqlCommand("SELECT \"YonetmenID\" FROM \"yonetmenTbl\" WHERE \"AdiSoyadi\" = @yonetmenAdi", baglanti))
            {
                yonetmenKomut.Parameters.AddWithValue("@yonetmenAdi", yonetmenAdi);
                object yonetmenID = yonetmenKomut.ExecuteScalar();
                return yonetmenID != null ? Convert.ToInt32(yonetmenID) : -1;
            }
        }

        // Tür adına göre TurID'yi bulan metod
        private int TurIDBul(string turAdi)
        {
            using (NpgsqlCommand turKomut = new NpgsqlCommand("SELECT \"TurID\" FROM \"TurTbl\" WHERE \"TurAdi\" = @turAdi", baglanti))
            {
                turKomut.Parameters.AddWithValue("@turAdi", turAdi);
                object turID = turKomut.ExecuteScalar();
                return turID != null ? Convert.ToInt32(turID) : -1;
            }
        }

        private void Guncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                if (int.TryParse(txtID.Text, out int id))
                {
                    // Yönetmen adını ve tür adını kullanarak ilgili ID'leri bul
                    int yonetmenID = YonetmenIDBul(txtYonetmen.Text);
                    int turID = TurIDBul(txtTur.Text);


                    // sql sorgusunu oluştur ve sadece dolu olan TextBox'ları güncelle
                    NpgsqlCommand komut2 = new NpgsqlCommand("UPDATE \"filmTbl\" SET " +
                                                            "\"Ad\"=@p2, " +
                                                            "\"Rating\"=@p3, " +
                                                            "\"Butce\"=@p4, " +
                                                            "\"YapimYili\"=@p5,  " +
                                                            "\"GiseSayısı\"=@p6, " +
                                                            "\"Afis\"=@p7, " +
                                                            "\"TurID\"=@p8, " +
                                                            "\"YonetmenID\"=@p9 " +
                                                            "WHERE \"FilmID\"=@p1", baglanti);

                    komut2.Parameters.AddWithValue("@p1", id);
                    komut2.Parameters.AddWithValue("@p2", NpgsqlDbType.Text, txtAd.Text);
                    komut2.Parameters.AddWithValue("@p3", NpgsqlDbType.Double, double.Parse(txtRating.Text));
                    komut2.Parameters.AddWithValue("@p4", NpgsqlDbType.Double, double.Parse(txtButce.Text));
                    komut2.Parameters.AddWithValue("@p5", NpgsqlDbType.Integer, int.Parse(txtYil.Text));
                    komut2.Parameters.AddWithValue("@p6", NpgsqlDbType.Integer, int.Parse(txtBoxGiseSayisi.Text));

                    byte[] resimByte;
                    if (!string.IsNullOrEmpty(txtResim.Text) && File.Exists(txtResim.Text))
                    {
                        resimByte = File.ReadAllBytes(txtResim.Text);
                        komut2.Parameters.AddWithValue("@p7", NpgsqlDbType.Bytea, resimByte);
                    }
                    else
                    {
                        komut2.Parameters.AddWithValue("@p7", NpgsqlDbType.Bytea, DBNull.Value);
                    }

                    komut2.Parameters.AddWithValue("@p8", NpgsqlDbType.Integer, turID); // Tür ID'sini kullan
                    komut2.Parameters.AddWithValue("@p9", NpgsqlDbType.Integer, yonetmenID); // Yönetmen ID'sini kullan

                    komut2.ExecuteNonQuery();

                    MessageBox.Show("Film güncelleme işlemi başarılı");
                    FilmleriGoster();
                }
                else
                {
                    MessageBox.Show("Geçersiz id formatı");
                }

                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); //bağlantıyı kapat
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata Oluştu");
            }
        }

        private void Sil_button_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                int filmID = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

                using (NpgsqlCommand silKomut = new NpgsqlCommand("DELETE FROM \"filmTbl\" WHERE \"FilmID\" = @filmID", baglanti))
                {
                    silKomut.Parameters.AddWithValue("@filmID", filmID);
                    silKomut.ExecuteNonQuery();
                }

                MessageBox.Show("Film silme işlemi başarılı");
                FilmleriGoster();

                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); // bağlantıyı kapat
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir film seçin.");
            }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtAd.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtRating.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtButce.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            txtYil.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            txtBoxGiseSayisi.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            txtTur.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            txtYonetmen.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();



            if (baglanti.State != ConnectionState.Open)
            {
                baglanti.Open(); // bağlantıyı aç
            }

            int turID = int.Parse(dataGridView1.CurrentRow.Cells[7].Value.ToString());
            int yonetmenID = int.Parse(dataGridView1.CurrentRow.Cells[8].Value.ToString());

            // Tur adını getir
            using (NpgsqlCommand turKomut = new NpgsqlCommand("SELECT \"TurAdi\" FROM \"TurTbl\" WHERE \"TurID\" = @turID", baglanti))
            {
                turKomut.Parameters.AddWithValue("@turID", turID);
                object turAd = turKomut.ExecuteScalar();
                txtTur.Text = turAd != null ? turAd.ToString() : string.Empty;
            }

            // Yönetmen adını getir
            using (NpgsqlCommand yonetmenKomut = new NpgsqlCommand("SELECT \"AdiSoyadi\" FROM \"yonetmenTbl\" WHERE \"YonetmenID\" = @yonetmenID", baglanti))
            {
                yonetmenKomut.Parameters.AddWithValue("@yonetmenID", yonetmenID);
                object yonetmenAd = yonetmenKomut.ExecuteScalar();
                txtYonetmen.Text = yonetmenAd != null ? yonetmenAd.ToString() : string.Empty;
            }


            int filmID = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());

            using (NpgsqlCommand komut5 = new NpgsqlCommand("SELECT * FROM \"filmTbl\" WHERE \"FilmID\" = @filmID", baglanti))
            {
                komut5.Parameters.AddWithValue("@filmID", filmID);

                using (NpgsqlDataReader dr = komut5.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        int filmPosterIndex = dr.GetOrdinal("Afis");

                        if (!dr.IsDBNull(filmPosterIndex))
                        {
                            byte[] resim = (byte[])dr[filmPosterIndex];
                            MemoryStream ms = new MemoryStream(resim);
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                        else
                        {
                            pictureBox1.Image = null;
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
            }
            if (baglanti.State == ConnectionState.Open)
            {
                baglanti.Close(); //bağlantıyı kapat
            }
        }

        private void btnFilmGoster_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                string gercekAd = textBoxOyuncuID.Text; // textboxa girilen gerçek ada göre arama

                NpgsqlCommand komut = new NpgsqlCommand("SELECT \"filmTbl\".\"FilmID\", \"filmTbl\".\"Ad\", " +
                                                       "\"TurTbl\".\"TurAdi\", \"yonetmenTbl\".\"AdiSoyadi\" " +
                                                       "FROM \"OyuncuFilm\" " +
                                                       "INNER JOIN \"filmTbl\" ON \"OyuncuFilm\".\"FilmID\" = \"filmTbl\".\"FilmID\" " +
                                                       "LEFT JOIN \"TurTbl\" ON \"filmTbl\".\"TurID\" = \"TurTbl\".\"TurID\" " +
                                                       "LEFT JOIN \"yonetmenTbl\" ON \"filmTbl\".\"YonetmenID\" = \"yonetmenTbl\".\"YonetmenID\" " +
                                                       "INNER JOIN \"oyuncuTbl\" ON \"OyuncuFilm\".\"OyuncuID\" = \"oyuncuTbl\".\"OyuncuID\" " +
                                                       "WHERE \"oyuncuTbl\".\"GercekAdiSoyadi\" = @gercekAd", baglanti);

                komut.Parameters.AddWithValue("@gercekAd", gercekAd);

                NpgsqlDataReader dataReader = komut.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                // DataGridView'e veri ekleme
                dataGridView3.DataSource = dataTable;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); // bağlantıyı kapat
                }
            }
        }


        private void buttonYonetmenFilmGoster_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                string yonetmenID = textBoxYonetmenID.Text; //textboxa girilen değeri değişkene ata

                NpgsqlCommand komut = new NpgsqlCommand("SELECT \"yonetmenTbl\".\"AdiSoyadi\", \"filmTbl\".\"FilmID\", \"filmTbl\".\"Ad\" " +
                                                        "FROM \"filmTbl\" " +
                                                        "INNER JOIN \"yonetmenTbl\" ON \"filmTbl\".\"YonetmenID\" = \"yonetmenTbl\".\"YonetmenID\" " +
                                                        "WHERE \"yonetmenTbl\".\"YonetmenID\" = @yonetmenID", baglanti);

                komut.Parameters.AddWithValue("@yonetmenID", int.Parse(yonetmenID));

                NpgsqlDataReader dataReader = komut.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                // DataGridView'e veri ekleme
               dataGridViewFilmYonetmen.DataSource = dataTable;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); // bağlantıyı kapat
                }
            }
        }

        

        private void buttonBirlikteOynayanOyuncular_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State != ConnectionState.Open)
                {
                    baglanti.Open(); // bağlantıyı aç
                }

                NpgsqlCommand komut = new NpgsqlCommand(
                    "SELECT \"oyuncuTbl\".\"OyuncuID\", \"oyuncuTbl\".\"GercekAdiSoyadi\", COUNT(\"filmTbl\".\"FilmID\") AS \"OynadigiFilmSayisi\" " +
                    "FROM \"OyuncuFilm\" " +
                    "INNER JOIN \"oyuncuTbl\" ON \"OyuncuFilm\".\"OyuncuID\" = \"oyuncuTbl\".\"OyuncuID\" " +
                    "INNER JOIN \"filmTbl\" ON \"OyuncuFilm\".\"FilmID\" = \"filmTbl\".\"FilmID\" " +
                    "GROUP BY \"oyuncuTbl\".\"OyuncuID\", \"oyuncuTbl\".\"GercekAdiSoyadi\" " +
                    "ORDER BY \"OynadigiFilmSayisi\" DESC", baglanti);

                NpgsqlDataReader dataReader = komut.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                // DataGridView'e veri ekleme
                dataGridViewBirlikteOynayanOyuncular.DataSource = dataTable;
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); // bağlantıyı kapat
                }
            }
        }

        private void buttonResim_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
            txtResim.Text = openFileDialog1.FileName;
        }
    }
}
