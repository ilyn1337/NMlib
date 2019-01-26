# NMlib
Kolaylaştırılmış hafıza düzenleme kütüphanesi. 

**Şu anlık 64 bit işletim sistemlerini destekler.**

Kullanım:
+ Projenizi oluşturun
+ Projeye "app.manifest" dosyası ekleyin ve izinleri "<requestedExecutionLevel  level="requireAdministrator" uiAccess="false" />" olarak değiştirin.
+ NMlib'i DLL yada Class olarak ekleyin, kodunuzun en üstüne:
C# için: using NMLib64;
VB.NET için: Imports NMlibVB
ekleyin.
+ Bit Timer oluşturun ve içine şu kodu yapıştırın: 
C# için: NeutronMemoryLibrary.IslemEkle("İŞLEM İSMİ");
VB.NET için: NMlib64VB.NeutronMemoryLibrary.IslemEkle("İŞLEM İSMİ")

**Opcode kullanımı**

Herhangi bir kontrolün içine yapıştırın:
C# için: NeutronMemoryLibrary.OpcodeKullan(0xOPCODE, "DEĞİŞTİRİLECEK BİT DEĞERLERİ");
VB.NET için: NMlib64VB.NeutronMemoryLibrary.OpcodeKullan(&HOPCODE, "DEĞİŞTİRİLECEK BİT DEĞERLERİ") '&H'yi silmeyin.

**Pointer Kullanımı**

Herhangi bir kontrolün içine yapıştırın:
C# için: NeutronMemoryLibrary.PointerKullan(0xPOINTER ADRESİ, 0xOFFSET1, 0xOFFSET2, 0xOFFSET3, DEĞİŞTİRİLECEK DEĞER);
VB.NET için: NMlib64VB.NeutronMemoryLibrary.PointerKullan(&HPOINTER ADRESİ, &HOFFSET1, &HOFFSET2, &HOFFSET13, DEĞİŞTİRİLECEK DEĞER)

