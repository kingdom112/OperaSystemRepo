using UnityEngine;
using System.Collections;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;

public class TestPDF : MonoBehaviour
{
   
    // Use this for initialization
    void Start()
    {


        string pdfPath = Application.persistentDataPath;
        DateTime dt = DateTime.Now;
        string fileName = pdfPath + "/" + dt.Year + "-" + dt.Month + "-" + dt.Day + "-" + dt.Hour + "-" + dt.Minute + "-" + dt.Second + ".pdf";

        Debug.Log(fileName);

        //使用 TTF 字体
        BaseFont bf = BaseFont.CreateFont("C:/Windows/Fonts/simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        // 创建 PDF 文档
        iTextSharp.text.Document document
            = new iTextSharp.text.Document(PageSize.A4, 60, 60, 72, 72);

        //创建PDF文档
        Document doc = new Document(PageSize.A4, 60, 60, 72, 72);
        //创建写入实例，PDF文档保存位置
        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
        //打开文档
        doc.Open();
        //创建摘要
        doc.AddTitle("");
        doc.AddAuthor("Addis Chen");
        doc.AddSubject("PDF Test");
        doc.AddCreator("PDF Test");


        //创建一个字体
        iTextSharp.text.Font testFont = new iTextSharp.text.Font(bf, 18);

        //创建一个段落
        Paragraph pdf_title = new Paragraph("Just A Test", testFont);
        pdf_title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
        doc.Add(pdf_title);
        
       // Image img = Image.GetInstance("");

        //关闭文档
        doc.Close();
        //打开pdf文件
        System.Diagnostics.Process.Start(fileName);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
