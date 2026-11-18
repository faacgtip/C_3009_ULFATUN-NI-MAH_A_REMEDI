using System;
using System.Collections.Generic;
using System.Globalization;

namespace MBG
{
    // 1. CLASS, OBJECT, ATTRIBUTE, DAN METHOD
    public class Sekolah
    {
        public string IdSekolah { get; private set; }
        public string NamaSekolah { get; private set; }
        public int JumlahPaketDiterima { get; private set; }

        public Sekolah(string idSekolah, string namaSekolah, int jumlahPaketDiterima)
        {
            IdSekolah = idSekolah;
            NamaSekolah = namaSekolah;
            JumlahPaketDiterima = jumlahPaketDiterima;
        }

        // Dipanggil oleh DistribusiMBG saat pengiriman berhasil
        public void TerimaPaket(int jumlah)
        {
            if (jumlah > 0)
                JumlahPaketDiterima += jumlah;
        }

        public void TampilInfo()
        {
            Console.WriteLine($"ID Sekolah           : {IdSekolah}");
            Console.WriteLine($"Nama Sekolah         : {NamaSekolah}");
            Console.WriteLine($"Jumlah Paket Diterima: {JumlahPaketDiterima}");
        }
    }

    // 2. ENCAPSULATION  +  5. ABSTRACT CLASS
    public abstract class PaketMBG
    {
        // Atribut private
        private string idPaket;
        private string namaPaket;
        private int stokDapur;

        // Properties
        public string IdPaket
        {
            get { return idPaket; }
            set { idPaket = value; }
        }

        public string NamaPaket
        {
            get { return namaPaket; }
            set { namaPaket = value; }
        }

        // Setter private: stok tidak bisa diubah/dikurangi langsung dari luar class.
        // Nilai negatif ditolak.
        public int StokDapur
        {
            get { return stokDapur; }
            private set
            {
                if (value < 0)
                    throw new ArgumentException("Stok dapur tidak boleh negatif.");
                stokDapur = value;
            }
        }

        protected PaketMBG(string idPaket, string namaPaket, int stokDapur)
        {
            IdPaket = idPaket;
            NamaPaket = namaPaket;
            StokDapur = stokDapur; // validasi negatif berjalan lewat setter
        }

        // Satu-satunya cara mengurangi stok.
        // Mengembalikan true jika berhasil, false jika jumlah tidak valid / melebihi stok.
        public bool KurangiStok(int jumlah)
        {
            if (jumlah <= 0)
                return false;
            if (jumlah > stokDapur)
                return false;

            StokDapur = stokDapur - jumlah;
            return true;
        }

        // Abstract method: wajib di-override oleh class turunan
        public abstract int HitungBiaya(int jumlah);

        public virtual void TampilInfo()
        {
            Console.WriteLine($"ID Paket  : {IdPaket}");
            Console.WriteLine($"Nama Paket: {NamaPaket}");
            Console.WriteLine($"Stok Dapur: {StokDapur}");
        }
    }

    // 3. INHERITANCE  +  4. POLYMORPHISM (override)
    public class PaketReguler : PaketMBG
    {
        public const int HargaPerPaket = 15000;
        public string Isi { get; } = "Nasi, ayam, sayur, dan buah";

        public PaketReguler(string idPaket, string namaPaket, int stokDapur)
            : base(idPaket, namaPaket, stokDapur)
        {
        }

        public override int HitungBiaya(int jumlah)
        {
            return jumlah * HargaPerPaket;
        }

        public override void TampilInfo()
        {
            base.TampilInfo();
            Console.WriteLine($"Isi Paket : {Isi}");
            Console.WriteLine($"Harga     : Rp{HargaPerPaket.ToString("N0", new CultureInfo("id-ID"))} per paket");
        }
    }

    public class PaketKhusus : PaketMBG
    {
        public const int HargaPerPaket = 20000;
        public string JenisDiet { get; set; }

        public PaketKhusus(string idPaket, string namaPaket, int stokDapur, string jenisDiet)
            : base(idPaket, namaPaket, stokDapur)
        {
            JenisDiet = jenisDiet;
        }

        public override int HitungBiaya(int jumlah)
        {
            return jumlah * HargaPerPaket;
        }

        public override void TampilInfo()
        {
            base.TampilInfo();
            Console.WriteLine($"Jenis Diet: {JenisDiet}");
            Console.WriteLine($"Harga     : Rp{HargaPerPaket.ToString("N0", new CultureInfo("id-ID"))} per paket");
        }
    }

    // 6. INTERFACE
    public interface DapatDidistribusikan
    {
        void KirimPaket();
    }

    public class DistribusiMBG : DapatDidistribusikan
    {
        public Sekolah SekolahTujuan { get; private set; }
        public PaketMBG Paket { get; private set; }
        public int Jumlah { get; private set; }

        public DistribusiMBG(Sekolah sekolahTujuan, PaketMBG paket, int jumlah)
        {
            SekolahTujuan = sekolahTujuan;
            Paket = paket;
            Jumlah = jumlah;
        }

        public void KirimPaket()
        {
            // Aturan 1: jumlah harus > 0
            if (Jumlah <= 0)
            {
                Console.WriteLine("Distribusi MBG gagal! Jumlah paket yang dikirim harus lebih dari 0.");
                return;
            }

            // Aturan 2: tidak boleh melebihi stok dapur
            if (Jumlah > Paket.StokDapur)
            {
                Console.WriteLine($"Distribusi MBG gagal! Jumlah paket yang dikirim ({Jumlah}) melebihi stok dapur yang tersedia ({Paket.StokDapur}).");
                return;
            }

            // Aturan 3: berhasil -> stok berkurang, sekolah menerima paket
            Paket.KurangiStok(Jumlah);
            SekolahTujuan.TerimaPaket(Jumlah);
            Console.WriteLine($"Distribusi MBG berhasil! Sebanyak {Jumlah} paket makanan telah dikirim ke sekolah {SekolahTujuan.NamaSekolah}");
        }
    }

    // 7. OPSIONAL - PengelolaMBG
    public class PengelolaMBG
    {
        // Hubungan PengelolaMBG dengan Sekolah adalah AGGREGATION:
        // - PengelolaMBG "memiliki" kumpulan Sekolah (has-a) dalam sebuah List.
        // - Objek Sekolah dibuat di luar PengelolaMBG (misalnya di Main) lalu dimasukkan
        //   lewat TambahSekolah(), sehingga Sekolah tetap bisa hidup sendiri meskipun
        //   PengelolaMBG dihapus.
        // - Bukan composition, karena Sekolah tidak dibuat dan tidak dimiliki penuh oleh
        //   PengelolaMBG (siklus hidupnya tidak bergantung pada PengelolaMBG).
        // - Bukan sekadar association, karena ada hubungan kepemilikan "whole-part"
        //   (PengelolaMBG sebagai whole, Sekolah sebagai part) yang disimpan sebagai koleksi.
        private List<Sekolah> daftarSekolah = new List<Sekolah>();
        private List<PaketMBG> daftarPaket = new List<PaketMBG>();

        public void TambahSekolah(Sekolah sekolah)
        {
            daftarSekolah.Add(sekolah);
        }

        public void TambahPaket(PaketMBG paket)
        {
            daftarPaket.Add(paket);
        }

        public void TampilkanDataMBG()
        {
            Console.WriteLine("===== DATA SEKOLAH PENERIMA MBG =====");
            foreach (Sekolah s in daftarSekolah)
            {
                s.TampilInfo();
                Console.WriteLine("-------------------------------------");
            }

            Console.WriteLine("===== DATA PAKET MBG & STOK DAPUR =====");
            foreach (PaketMBG p in daftarPaket)
            {
                p.TampilInfo();
                Console.WriteLine("-------------------------------------");
            }
        }
    }

    // MAIN
    internal class Program
    {
        static void Main(string[] args)
        {
            CultureInfo id = new CultureInfo("id-ID");

            // ---- 1. Class & Object ----
            Console.WriteLine("=== 1. CLASS & OBJECT ===");
            Sekolah sekolah1 = new Sekolah("S001", "SDN 1 Jember", 0);
            Sekolah sekolah2 = new Sekolah("S002", "SMPN 3 Jember", 0);
            sekolah1.TampilInfo();
            Console.WriteLine();
            sekolah2.TampilInfo();
            Console.WriteLine();

            // ---- 2/3. Encapsulation & Inheritance ----
            // PaketMBG adalah abstract -> tidak bisa: new PaketMBG(...)
            Console.WriteLine("=== 2 & 3. ENCAPSULATION & INHERITANCE ===");
            PaketReguler reguler = new PaketReguler("P001", "Paket Reguler", 100);
            PaketKhusus khusus = new PaketKhusus("P002", "Paket Khusus", 50, "Rendah Gula");
            reguler.TampilInfo();
            Console.WriteLine();
            khusus.TampilInfo();
            Console.WriteLine();

            // reguler.StokDapur = 5;   // ERROR: setter private, tidak bisa diubah dari luar
            Console.WriteLine("Mengurangi stok reguler 10 lewat KurangiStok(10): " + reguler.KurangiStok(10));
            Console.WriteLine("Stok reguler sekarang: " + reguler.StokDapur);
            Console.WriteLine("Mengurangi stok reguler 999 (melebihi stok): " + reguler.KurangiStok(999));
            Console.WriteLine("Mengurangi stok reguler -5 (tidak valid)   : " + reguler.KurangiStok(-5));
            Console.WriteLine("Stok reguler tetap: " + reguler.StokDapur);
            Console.WriteLine();

            // ---- 4/5. Polymorphism ----
            Console.WriteLine("=== 4 & 5. POLYMORPHISM & ABSTRACT CLASS ===");
            PaketMBG[] daftarPaketPoly = { reguler, khusus };
            foreach (PaketMBG p in daftarPaketPoly)
            {
                int biaya = p.HitungBiaya(10);
                Console.WriteLine($"{p.NamaPaket} x 10 = Rp{biaya.ToString("N0", id)}");
            }
            Console.WriteLine();

            // ---- 6. Interface ----
            Console.WriteLine("=== 6. INTERFACE (DISTRIBUSI MBG) ===");
            DapatDidistribusikan d1 = new DistribusiMBG(sekolah1, reguler, 20);   // berhasil
            DapatDidistribusikan d2 = new DistribusiMBG(sekolah2, khusus, 100);   // gagal: melebihi stok
            DapatDidistribusikan d3 = new DistribusiMBG(sekolah2, khusus, 0);     // gagal: jumlah 0
            DapatDidistribusikan d4 = new DistribusiMBG(sekolah2, khusus, 15);    // berhasil

            d1.KirimPaket();
            d2.KirimPaket();
            d3.KirimPaket();
            d4.KirimPaket();
            Console.WriteLine();

            // ---- 7. Opsional: PengelolaMBG ----
            Console.WriteLine("=== 7. PENGELOLA MBG ===");
            PengelolaMBG pengelola = new PengelolaMBG();
            pengelola.TambahSekolah(sekolah1);
            pengelola.TambahSekolah(sekolah2);
            pengelola.TambahPaket(reguler);
            pengelola.TambahPaket(khusus);
            pengelola.TampilkanDataMBG();

            Console.WriteLine("Tekan tombol apa saja untuk keluar...");
            Console.ReadKey();
        }
    }
}