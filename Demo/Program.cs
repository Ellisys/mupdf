using MuPDFCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Demo
{
    class Program
    {
        static void Main()
        {
            using MuPDFContext ctx = new MuPDFContext();
            DirectoryInfo d = new DirectoryInfo("C:/PatentOffice/PatentOffice/Temp/mupdfcore/Demo/");

            Parallel.ForEach(d.GetFiles("*.pdf"), file =>
            {
                string nameFile = file.Name;

                Console.WriteLine($"list files: {nameFile}");

                var list = Task.Run(() =>
                {
                    using MuPDFDocument doc1 = new MuPDFDocument(ctx, nameFile);
                    Console.WriteLine($"Task beginning {nameFile}");

                    for (int i = 0; i < doc1.Pages.Count; i++)
                    {
                        Console.WriteLine($"{nameFile}");

                        string TrimNameFile = nameFile.Replace(".pdf", "");
                        string nameImg = $"{TrimNameFile}-page{i}.png";

                        doc1.SaveImage(i, 3, PixelFormats.RGBA, nameImg, RasterOutputFileTypes.PNG);

                        Console.WriteLine($"{nameFile} SaveImage");

                        using MuPDFDocument doc3 = new MuPDFDocument(ctx, nameImg);

                        Console.WriteLine($"{nameFile} GetImage");

                        MuPDFStructuredTextPage page = doc3.GetStructuredTextPage(0, new TesseractLanguage(TesseractLanguage.Fast.Eng));

                        Console.WriteLine("------------------------------------------------------------------------------");
                        Console.WriteLine($"START {nameImg} OF {nameFile} ");
                        Console.WriteLine("------------------------------------------------------------------------------");

                        Console.WriteLine($"{nameFile} Extract Text");
                        Console.WriteLine($" ");

                        foreach (MuPDFStructuredTextBlock blk in page)
                        {
                            foreach (MuPDFStructuredTextLine line in blk)
                            {
                                System.Console.WriteLine(line.Text);
                            }
                        }
                        Console.WriteLine("------------------------------------------------------------------------------");
                        Console.WriteLine($"END PAGE{i} OF {nameFile} ");
                        Console.WriteLine("------------------------------------------------------------------------------");
                    }
                });
                list.Wait();
                Console.WriteLine($"Task {nameFile} completed.");
            });
        }
    }
};
