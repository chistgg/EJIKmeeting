using Meeting.Models;
using Ninject;
using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Meeting.Models.Repository;
using Meeting.Helpers;

namespace Meeting.Protocol
{
    public class GeneratePDF : IProtocolGenerate
    {
        private Guid MeetingID { get; set; }
        private string OutFilePath { get; set; }
        private Font HeaderFont { get; set; }
        private Font MainFont { get; set; }
        private Font AdditionalFont { get; set; }
        private Font LinkFont { get; set; }

        [Inject]
        public MeetingContainer Model { get; set; }

        public void InitializeParam(Guid meetingID, string outFilePath)
        {
            MeetingID = meetingID;
            OutFilePath = outFilePath;
            MainFont = GetRussianFont(12, BaseColor.BLACK);
            HeaderFont = GetRussianFont(14, BaseColor.BLACK);
            AdditionalFont = GetRussianFont(10, BaseColor.BLACK);
            LinkFont = GetRussianFont(12, BaseColor.BLUE);
        }

        private Font GetRussianFont(int size, BaseColor color)
        {
            string fg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.TTF");
            BaseFont fgBaseFont = BaseFont.CreateFont(fg, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Font(fgBaseFont, size, Font.NORMAL, color);
        }

        public void GenerateProtocol()
        {
            if (OutFilePath == null)
                throw new NullReferenceException("You should initialize OutFilePath property for creating the document.");
            if (MeetingID == null)
                throw new NullReferenceException("You should initialize MeetingID property for creating the document.");

            GenerateDocument();
        }

        public void GenerateDocument()
        {
            var doc1 = new Document();
            //use a variable to let my code fit across the page...
            PdfWriter.GetInstance(doc1, new FileStream(OutFilePath, FileMode.Create));

            doc1.Open();
            doc1.Add(GenerateTable());
            doc1.Close();
        }

        private PdfPTable GenerateTable()
        {
            Chat meeting = ChatRepository.GetChatByID(MeetingID, Model);
            List<Message> messages = MessageRepository.GetInProtocolMessages(MessageRepository.GetAllMessages(MeetingID, Model));
            List<User> users = UserRepository.GetChatUsers(MeetingID, Model).Where(u => UserStatusHelper.WasInChat(u.Status)).ToList();

            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 2f, 5f, 1f };
            table.SetWidths(widths);
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            table.AddCell(GetCell("Протокол совещания", HeaderFont, 3, 1, 1));
            table.AddCell(GetCell("Тема совещания: " + meeting.Name, 3));
            table.AddCell(GetCell("Начало совещания: " + StringHelper.DateTimeToString(meeting.StartingTime), 3));
            table.AddCell(GetCell("Конец совещания: " + StringHelper.DateTimeToString(MessageRepository.GetLastMessageDateTime(messages)), 3));

            foreach (var u in users)
            {
                table.AddCell(GetCell(u.Name));
            }

            foreach (var m in messages)
            {
                table.AddCell(GetCell(m.User.Name + "\n" + StringHelper.DateTimeToString(m.SendingTime)));
                if (m.File != null)
                    table.AddCell(GetCell(m.Content + "\n" + StringHelper.GetPublicDownloadURL(m.File.ID)));
                else
                    table.AddCell(GetCell(m.Content));
                table.AddCell(GetCell("QR requared"));
            }
            table.KeepTogether = false;
            return table;
        }

        private PdfPCell GetCell(string content, int colspan = 1, int rowspan = 1, int alignment = 0)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, MainFont));
            cell.Colspan = colspan;
            cell.Rowspan = rowspan;
            cell.HorizontalAlignment = alignment; //0=Left, 1=Centre, 2=Right

            return cell;
        }

        private PdfPCell GetCell(string content, Font font, int colspan = 1, int rowspan = 1, int alignment = 0)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.Colspan = colspan;
            cell.Rowspan = rowspan;
            cell.HorizontalAlignment = alignment; 

            return cell;
        }

    }
}