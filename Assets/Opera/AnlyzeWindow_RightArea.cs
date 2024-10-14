using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;

public class AnlyzeWindow_RightArea : RightStackMember
{
    public GUISkin skin;  
    public List<Texture2D> texs = new List<Texture2D>();
    public ScreenCapturer screenCapturer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 开始截图
    /// </summary>
    public void StartScreenCapture()
    { 
        screenCapturer.SetUpController(this);
        screenCapturer.SetUse(true);

    }

    private void SetUseAllScreenMask(bool _use)
    {
        screenCapturer.VFPlayer.SetUseAllScreenMask(_use);
    }
    public void OpenEditWindow()
    {
        showEditWindow = true;
        SetUseAllScreenMask(true);
    } 

    bool CheckCanWrite()
    {
        for(int i=0; i<printClasses.Count; i++)
        {
            if(printClasses[i].pictureNum >=0 && printClasses[i].pictureNum < texs.Count)
            {

            }
            else
            {
                return false;
            }
        }
        return true;
    }
    void WriteToPDF()
    {
        if (CheckCanWrite() == false) return;

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
        doc.AddAuthor("JTAOO Test");
        doc.AddSubject("JTAOO Test");
        doc.AddCreator("JTAOO Test");

        //创建一个字体
        iTextSharp.text.Font testFont = new iTextSharp.text.Font(bf, 18);

        Paragraph pdf_title = new Paragraph(pdfTitle, testFont);
        pdf_title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
        doc.Add(pdf_title);

        for (int i=0; i< printClasses.Count; i++)
        {
            if(printClasses[i].type == PrintClass.Type.Text)
            {
                Paragraph texParagraph = new Paragraph(printClasses[i].textContent, testFont);
                texParagraph.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                doc.Add(texParagraph);
            }
            else if(printClasses[i].type == PrintClass.Type.Picture)
            {
                Paragraph texParagraph = new Paragraph(printClasses[i].textContent, testFont);
                texParagraph.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                doc.Add(texParagraph);
                 
                Texture2D tex = texs[printClasses[i].pictureNum];
                string TexName = pdfPath + "/" + dt.Year + "-" + dt.Month + "-" + dt.Day + "-" + dt.Hour + "-" + dt.Minute + "-" + dt.Second + "_TEX.png";
                byte[] bytes = tex.EncodeToPNG();
                System.IO.File.WriteAllBytes(TexName, bytes);
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(TexName);
                img.ScaleToFit(595, 842);
                img.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(img);
                System.IO.File.Delete(TexName);
            }
            else if (printClasses[i].type == PrintClass.Type.Space)
            {
                Paragraph texParagraph = new Paragraph("          ", testFont);
                texParagraph.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                doc.Add(texParagraph);
            }
        }
       

       /* //创建一个段落
        Paragraph pdf_title = new Paragraph("示例PDF文档", testFont);
        pdf_title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
        doc.Add(pdf_title);

        //创建一个段落
        Paragraph texParagraph = new Paragraph("图片示例：", testFont);
        texParagraph.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
        doc.Add(texParagraph);

        for(int i=0; i<texs.Count; i++)
        {
            Texture2D tex = texs[i];
            string TexName = pdfPath + "/" + dt.Year + "-" + dt.Month + "-" + dt.Day + "-" + dt.Hour + "-" + dt.Minute + "-" + dt.Second + "_TEX.png";
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(TexName, bytes);
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(TexName);
            img.ScaleToFit(595, 842);
            img.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            doc.Add(img);
            System.IO.File.Delete(TexName);
        }
       


        //创建一个段落
        Paragraph nextParagraph = new Paragraph("注释示例：这是一行注释xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", testFont);
        nextParagraph.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
        doc.Add(nextParagraph);*/

        //关闭文档
        doc.Close();

        //打开文件夹
        string expPath = pdfPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", expPath);

        //打开pdf文件
        System.Diagnostics.Process.Start(fileName);
        
    }

    IEnumerator CaptureIE()
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = CaptureScreenshot(); 
        texs.Add(tex);
        WriteToPDF();
    }
    IEnumerator CaptureIE(Rect _rect)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = CaptureScreenshot(_rect);
        texs.Add(tex); 
    }
    /// <summary>
    /// 传递鼠标框选的截图Rect区域
    /// </summary>
    /// <param name="_rect"></param>
    public void PassCaptureScreenshotRect(Rect _rect)
    {
        screenCapturer.SetUse(false);
        StartCoroutine(CaptureIE(_rect));
    }
    /// <summary>
    /// 用默认区域截图
    /// </summary>
    /// <returns></returns>
    Texture2D CaptureScreenshot()
    {
        return CaptureScreenshot(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, Screen.width * 0.6f, Screen.height * 0.7f));
        //return CaptureScreenshot(new Rect(0, 0, Screen.width, Screen.height));
    }
    Texture2D CaptureScreenshot(Rect rect)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        return screenShot;
    }



    public class PrintClass
    {
        public enum Type
        {
            Text,
            Picture,
            Space
        }
        public Type type = Type.Text;
        public string textContent = "";
        public int pictureNum = 0;
        public PrintClass()
        {
            type = Type.Text;
            textContent = "";
        }
    }
    private List<PrintClass> printClasses = new List<PrintClass>();
    private string pdfTitle = "No Title";
    private int changeTypeNum = -1;
    private int changePictureSelectNum = -1;
    Vector2 scrollPos = Vector2.zero;
    private Rect windowRect = new Rect(20, 20,   1208,   925);
    private bool showEditWindow = false;
    private void OnGUI()
    {
        GUI.skin = skin;
        if (screenCapturer.Using)//screenCapturer使用时要隐藏编辑窗口
            return;
        if (showEditWindow == false)
            return;
        windowRect = GUI.Window(0, windowRect, Mywindowfunc, "编辑窗口");
        windowRect.width = Screen.width / 1600f * 1208f;
        windowRect.height = Screen.height / 900f * 825f;
    }
    void Mywindowfunc(int windowId)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("","CloseWindowButton", GUILayout.Width(40)))
        {
            showEditWindow = false;
            SetUseAllScreenMask(false);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(40);
       
        GUILayout.Space(20);
        GUILayout.Label("标题:");
        GUILayout.BeginHorizontal();
        GUILayout.Space(7);
        pdfTitle = GUILayout.TextField(pdfTitle);
        GUILayout.EndHorizontal();

        GUILayout.Space(20); 
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
      
        for(int i=0; i<printClasses.Count; i++)
        {
            PrintClass printOne = printClasses[i];
            GUILayout.BeginHorizontal();
            GUILayout.Space(7);
            if(GUILayout.Button("X  <color=#232d8f>元素." + (i + 1) + "</color>", "classesCloseButton"))
            {
                printClasses.RemoveAt(i);
                return;
            } 
            GUILayout.FlexibleSpace(); 
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            GUILayout.Label("Type:");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button(printOne.type.ToString(), "dropdownStyle", GUILayout.Width(100)))
            {
                changeTypeNum = i;
            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            if(changeTypeNum == i)//Type select
            {
                for (int j=0; j<3; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(((PrintClass.Type)j).ToString(), "dropdown_listStyle", GUILayout.Width(100)))
                    {
                        printOne.type = (PrintClass.Type)j;
                        changeTypeNum = -1;
                    }
                    GUILayout.Space(5);
                    GUILayout.EndHorizontal();
                }
            }
            if (printOne.type == PrintClass.Type.Text)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(0);
                GUILayout.Label("Text:");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(7);
                if(string.IsNullOrEmpty(printOne.textContent))
                {
                    printOne.textContent = "\n\n\n";
                }
                printOne.textContent = GUILayout.TextArea(printOne.textContent);
                GUILayout.EndHorizontal();
            }
            else if(printOne.type == PrintClass.Type.Picture)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(0);
                GUILayout.Label("Text:");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(7);
                if (string.IsNullOrEmpty(printOne.textContent))
                {
                    printOne.textContent = "\n\n\n";
                }
                printOne.textContent = GUILayout.TextArea(printOne.textContent);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(0);
                if (printOne.pictureNum >= 0 && printOne.pictureNum < texs.Count)
                {
                    GUILayout.Label("Picture:");
                }
                else
                {
                    GUILayout.Label("<color=red>Picture:</color>");
                }
                GUILayout.FlexibleSpace();
                if(GUILayout.Button((printOne.pictureNum + 1).ToString(), "dropdownStyle", GUILayout.Width(100)))
                {
                    if(texs.Count > 0) changePictureSelectNum = i;
                }
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                if(changePictureSelectNum == i)//picture Select
                { 
                    for (int k=0; k<texs.Count; k++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button((k+1).ToString(), "dropdown_listStyle", GUILayout.Width(100)))
                        {
                            printOne.pictureNum = k;
                            changePictureSelectNum = -1;
                        }
                        GUILayout.Space(10);
                        GUILayout.EndHorizontal();
                    } 
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(7);
                if(printOne.pictureNum >= 0 && printOne.pictureNum < texs.Count)
                {
                    GUILayout.Box(texs[printOne.pictureNum]);
                }
                else
                {
                    GUILayout.Box("<color=red>NoPictureSelected.</color>");
                }
               
                GUILayout.EndHorizontal();
            }
            else if (printOne.type == PrintClass.Type.Space)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(0);
                GUILayout.Label("Space");
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(30);
        }
         
        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("", "AddButton", GUILayout.Width(60)))
        {
            printClasses.Add(new PrintClass());
        }
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("", "ConfirmButton", GUILayout.Width(60)))
        {
            StartCoroutine(CaptureIE());
        }
        GUILayout.EndHorizontal(); 

        GUI.DragWindow(new Rect(0, 0, 1000, 1000));
    }
}
