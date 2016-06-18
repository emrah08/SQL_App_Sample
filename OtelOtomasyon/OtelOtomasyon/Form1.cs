using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace OtelOtomasyon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        } 

        //Veritabanıyla bağlantı kuruluyor.
        SqlConnection baglanti = new SqlConnection("Data Source=.\\;AttachDbFilename=" + Application.StartupPath + "\\otelmüşteri.mdf; Integrated Security = True;User Instance = True");
        
        //veritabanındaki veriler alınarak Listview'a ekleniyor.
        private void vericek()
        {
            listView1.Items.Clear();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select *From bilgiler", baglanti);
            SqlDataReader oku = komut.ExecuteReader();

            while (oku.Read())
            {
                ListViewItem ekle = new ListViewItem();
                ekle.Text = oku["id"].ToString();
                ekle.SubItems.Add(oku["ad"].ToString());
                ekle.SubItems.Add(oku["soyad"].ToString());
                ekle.SubItems.Add(oku["tckimlik"].ToString());
                ekle.SubItems.Add(oku["telefon"].ToString());
                ekle.SubItems.Add(oku["oda"].ToString());
                ekle.SubItems.Add(oku["gtarih"].ToString());
                ekle.SubItems.Add(oku["ctarih"].ToString());
                ekle.SubItems.Add(oku["tutar"].ToString());

                listView1.Items.Add(ekle);
            }
            idSort();
            baglanti.Close();
        }

        //veriler listeleniyor.
        private void btnGoruntule_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            vericek();

            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("Görüntülenecek kayıt bulunamadı.");
            }           
        }

        //Girilen veriler kaydediliyor.
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            //Bilgi girişi sırasında alanlardan herhangi biri boş bırakıldığında uyarı vermesi için ilgili kodlama yapılıyor.
            if ((tbxAd.Text == "") && (tbxSoyad.Text == "") && (tbxTC.Text == "") && (tbxTelefon.Text == "") && (tbxTutar.Text == "") && (cboxOda.SelectedText == ""))
            {
                MessageBox.Show("Lütfen bütün alanları doldurun.");
            }

            else
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("Insert into bilgiler (ad,soyad,tckimlik,telefon,oda,gtarih,ctarih,tutar) Values ('" + tbxAd.Text.ToString() + "', '" + tbxSoyad.Text.ToString() + "', '" + tbxTC.Text.ToString() + "', '" + tbxTelefon.Text.ToString() + "', '" + cboxOda.Text.ToString() + "', '" + dtimeGiris.Text.ToString() + "', '" + dtimeCikis.Text.ToString() + "', '" + tbxTutar.Text.ToString() + "')", baglanti);
                komut.ExecuteNonQuery();
                komut.CommandText = ("Insert into dolu_oda(doluyerler) Values ('" + cboxOda.Text + "')");
                komut.ExecuteNonQuery();
                komut.CommandText = "Delete From bos_oda Where bosyerler = '" + cboxOda.Text + "'";
                komut.ExecuteNonQuery();

                baglanti.Close();

                odaPasif();

                Temizle();

                vericek();

            }
        }

        //Listeden seçilen bilgileri
        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            if ((tbxAd.Text == "") && (tbxSoyad.Text == "") && (tbxTC.Text == "") && (tbxTelefon.Text == "") && (tbxTutar.Text == "") && (cboxOda.SelectedText == ""))
            {
                MessageBox.Show("Lütfen düzenlemek için listeden bir öğeye çift tıklayın.");
                vericek();
            }

            else
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("Update bilgiler set ad= '" + tbxAd.Text.ToString() + "', soyad= '" + tbxSoyad.Text.ToString() + "', tckimlik= '" + tbxTC.Text.ToString() + "', telefon= '" + tbxTelefon.Text.ToString() + "', oda= '" + cboxOda.Text.ToString() + "', gtarih= '" + dtimeGiris.Text.ToString() + "', ctarih= '" + dtimeCikis.Text.ToString() + "', tutar= '" + tbxTutar.Text.ToString() + "'", baglanti);
                komut.ExecuteNonQuery();
                baglanti.Close();

                vericek();
            }
        }

        int oda = 0;//Listeden seçilen satırı tespit etmek için oda numarası alnıyor.Seçilen satırdaki oda numarasına göre bilgiler ekrana getiriliyor.

        //Listeden seçilen sıranın bilgileri ilgili alanlara aktarılıyor.
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            oda = int.Parse(listView1.SelectedItems[0].SubItems[5].Text);

        
            tbxAd.Text = listView1.SelectedItems[0].SubItems[1].Text;
            tbxSoyad.Text = listView1.SelectedItems[0].SubItems[2].Text;
            tbxTC.Text = listView1.SelectedItems[0].SubItems[3].Text;
            tbxTelefon.Text = listView1.SelectedItems[0].SubItems[4].Text;
            cboxOda.Text = listView1.SelectedItems[0].SubItems[5].Text;
            dtimeGiris.Text = listView1.SelectedItems[0].SubItems[6].Text;
            dtimeCikis.Text = listView1.SelectedItems[0].SubItems[7].Text;
            tbxTutar.Text = listView1.SelectedItems[0].SubItems[8].Text;


        }

        //Veritabanında isme göre arama işlemi tanımlanıyor.
        private void btnAra_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select *From bilgiler Where ad like '%" + tbxAra.Text + "%'", baglanti);
            SqlDataReader oku = komut.ExecuteReader();

            if (tbxAra.Text == "")
            {
                MessageBox.Show("Aradığınız isim bulunamadı.");
            }
            else
            {
                while (oku.Read())
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["id"].ToString();
                    ekle.SubItems.Add(oku["ad"].ToString());
                    ekle.SubItems.Add(oku["soyad"].ToString());
                    ekle.SubItems.Add(oku["tckimlik"].ToString());
                    ekle.SubItems.Add(oku["telefon"].ToString());
                    ekle.SubItems.Add(oku["oda"].ToString());
                    ekle.SubItems.Add(oku["gtarih"].ToString());
                    ekle.SubItems.Add(oku["ctarih"].ToString());
                    ekle.SubItems.Add(oku["tutar"].ToString());

                    listView1.Items.Add(ekle);

                }

                if (listView1.Items.Count == 0)
                    MessageBox.Show("Aradığınız isim bulunamadı.");
            }

            baglanti.Close();
        }

        //Seçilen bilgilerin silinmesi işlemi tanımlanıyor.
        private void btnSil_Click(object sender, EventArgs e)
        {
            if ((tbxAd.Text == "") && (tbxSoyad.Text == "") && (tbxTC.Text == "") && (tbxTelefon.Text == "") && (tbxTutar.Text == "") && (cboxOda.SelectedText == ""))
            {
                MessageBox.Show("Lütfen düzenlemek için listeden bir öğeye çift tıklayın.");
                vericek();
            }

            else
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("Delete From bilgiler Where oda = (" + oda + ")", baglanti);
                komut.ExecuteNonQuery();
                //Silinen odanın numarası boş_oda tablosuna gönderilip, dolu_oda tablosundan silinerek tekrar kullanılabilir hale getiriliyor.
                komut.CommandText = "Insert into bos_oda(bosyerler) Values('" + cboxOda.Text + "')";
                komut.ExecuteNonQuery();
                komut.CommandText = "Delete From dolu_oda Where doluyerler = " + cboxOda.Text + "";
                komut.ExecuteNonQuery();

                baglanti.Close();

                odaAktif();

                OdaSirala();

                Temizle();

                vericek();
            }
        }

        //Formda ki Butonlar bir Button dizisine alınarak programın açılışı sırasında aktif olanların Yeşil, pasif olanların ise Kırmızı olması sağlanıyor.
        private void Form1_Load(object sender, EventArgs e)
        {
            
            OdaSirala();

              Button[] buton = {btnOda1, btnOda2, btnOda3, btnOda4, btnOda5, btnOda6, btnOda7, btnOda8, btnOda9};

              for (int i = 1; i < 9; i++)
              {
                  if (!cboxOda.Items.Contains(i.ToString()))
                  {
                      buton[i - 1].BackColor = Color.Red;
                      buton[i - 1].Enabled = false;
                  }
              }             
        }


        //Kaydetme işleminde Combobox'ta seçilen oda numarasına karşılık gelen Buton döngü ile pasif hale getirilip, rengi Kırmızı yapılıyor.
        private void odaPasif()
        {
            string oda = cboxOda.SelectedItem.ToString().TrimEnd();

            Button[] buton = { btnOda1, btnOda2, btnOda3, btnOda4, btnOda5, btnOda6, btnOda7, btnOda8, btnOda9 };

            for(int i = 0; i<9; i++)
            {
                if(oda == buton[i].Text)
                {
                    buton[i].BackColor = Color.Red;
                    buton[i].Enabled = false;
                }
            }
        }

        //Silme işleminde Combobox'ta seçilen oda numarasına karşılık gelen Buton döngü ile aktif hale getirilip, rengi yeşil yapılıyor.
        private void odaAktif()
        {
            
            string oda = cboxOda.Text.TrimEnd();

            Button[] buton = { btnOda1, btnOda2, btnOda3, btnOda4, btnOda5, btnOda6, btnOda7, btnOda8, btnOda9 };

            for (int i = 0; i < 9; i++)
            {
                if (oda == buton[i].Text)
                {
                    buton[i].BackColor = Color.LimeGreen;
                    buton[i].Enabled = true;
                }
            }
        }
          
        //cboxOda isimli Combobox'in içeriği her işlemden sonra yeniden sıralanıyor.  
        private void OdaSirala()
        {
            cboxOda.Items.Clear();

            //Güncel boş oda verisi alınıp.Combobox'a aktarılıyor.
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select *From bos_oda", baglanti);
            SqlDataReader oku = komut.ExecuteReader();


            while (oku.Read())
            {
                cboxOda.Items.Add(oku[0].ToString().TrimEnd());
            }
            baglanti.Close();

            //Toplam boş oda sayısı alınarak, oda sayısı büyüklüğünde bir dizi oluşturuluyor.
            int odasayisi = cboxOda.Items.Count;
            string[] odalar = new string[odasayisi];

            //oda numaraları diziye aktarılıp sonra sıralanıyor.Ardından sıralı ahli yeniden Combobox'a aktarılıyor.
            for(int i = 0; i<odasayisi; i++)
            {
                odalar[i] = cboxOda.Items[i].ToString();
            }

            Array.Sort(odalar);

            for(int i = 0; i<odasayisi; i++ )
            {
                cboxOda.Items[i] = odalar[i];
            }
        }
        
        //Bütün Kontroller temizleniyor.
        private void Temizle()
        {
            tbxAd.Clear();
            tbxSoyad.Clear();
            tbxTC.Clear();
            tbxTelefon.Clear();
            tbxTutar.Clear();
            cboxOda.ResetText();
            dtimeGiris.ResetText();
            dtimeCikis.ResetText();
        }

        //Eğer oda seçimi sırasında birden fazla butona basılırsa, bir önceki butonun eski haline gelmesi için bir önceki butonun Text değeri tutuluyor.
        int butonNo = 0;

        //Oda butonların hepsi bu Click olayına bağlı.
        private void btnOda_Click(object sender, EventArgs e)
        {
            Button[] buton = { btnOda1, btnOda2, btnOda3, btnOda4, btnOda5, btnOda6, btnOda7, btnOda8, btnOda9 };
            Button btn = new Button();
            btn = (Button)sender;

            
            cboxOda.ResetText();

            //Farklı bir oda numarasına tıklanması durumunda bir önceki oda numarası tekrar aktif hale getiriliyor.
            for (int i = 0; i < 9; i++)
            {
                if (buton[i].Text == butonNo.ToString())
                {
                    buton[i].BackColor = Color.LimeGreen;
                    buton[i].Enabled = true;
                }

            }

            //Tıklanan butonun Text değeri Combobox'a aktarılıyor.Ayrıca buton pasif yapılıarak rengi Kırımızı yapılıyor.
            for (int i = 1; i<=9; i++)
            {
                if (btn.Text == i.ToString())
                {
                    cboxOda.SelectedItem = i.ToString();
                    btn.Enabled = false;
                    btn.BackColor = Color.Red;
                    cboxOda.Focus();
                    butonNo = i;
                }
            }           
        }

        //Listede ki sıra numaraları yeniden sıralanıyor.
        private void idSort()
        {
            for(int i = 0; i<listView1.Items.Count; i++)
            {
                listView1.Items[i].SubItems[0].Text = (i + 1).ToString();
            }
        }

        //İlgili Texboxlara sadece harf girişinin yapılabilmesi sağlanıyor.
        private void SadeceHarf(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                        && !char.IsSeparator(e.KeyChar) || char.IsWhiteSpace(e.KeyChar);
        }

        //İlgili Texboxlara sadece rakam girişinin yapılabilmesi sağlanıyor.
        private void SadeceRakam(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) || char.IsWhiteSpace(e.KeyChar);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
