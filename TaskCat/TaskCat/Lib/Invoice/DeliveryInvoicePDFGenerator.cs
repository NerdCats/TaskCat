﻿namespace TaskCat.Lib.Invoice
{
    using Data.Lib.Invoice.Response;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Data.Model.Inventory;
    using System.Collections.Generic;
    using System.IO;

    public class DeliveryInvoicePDFGenerator : IPDFService<DeliveryInvoice>
    {
        public void GeneratePDF(DeliveryInvoice invoice)
        {
            PdfPTable itemsTable = GenerateItemsTable(invoice);

            FileStream fileStream = new FileStream(invoice.HRID + ".pdf",
                                                    FileMode.Create,
                                                    FileAccess.Write,
                                                    FileShare.None);
            // Generating a PDF document
            using (Document doc = new Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
                doc.Open();

                //Title
                Paragraph title = new Paragraph(invoice.HRID);
                title.Alignment = 2;

                Paragraph date = new Paragraph("Date: " + invoice.CreatedTime.Value.ToShortDateString());
                date.Alignment = 2;

                doc.Add(title);
                doc.Add(date);

                //Company Address
                // TODO: Need to fix this, Issue #27
                Paragraph companyName = new Paragraph("GO! Fetch");
                doc.Add(companyName);

                doc.Add(new Paragraph("[Address Line 1]"));
                doc.Add(new Paragraph("[Address Line 2]"));
                doc.Add(new Paragraph("[Town/City]"));
                doc.Add(new Paragraph("[Postcode]"));
                doc.Add(new Paragraph("Phone: [Phone Number]"));
                doc.Add(new Paragraph("__________________________"));

                if (invoice.BillingAddress == invoice.ShippingAddress)
                {
                    doc.Add(new Paragraph("Billing / Shipping Address:"));
                }
                else
                {
                    doc.Add(new Paragraph("Shipping Address:"));
                }

                doc.Add(new Paragraph(invoice.CustomerName));
                doc.Add(new Paragraph(invoice.ShippingAddress.Address));

                if (invoice.BillingAddress != invoice.ShippingAddress)
                {
                    doc.Add(new Paragraph("Billing Address:"));
                    doc.Add(new Paragraph(invoice.CustomerName));
                    doc.Add(new Paragraph(invoice.BillingAddress.Address));
                }

                Paragraph separator = new Paragraph("_____________________________________________________________________________      ");
                separator.SpacingAfter = 5.5f;
                doc.Add(separator);

                //Table and total
                doc.Add(itemsTable);
                doc.Close();
            }
        }

        private PdfPTable GenerateItemsTable(DeliveryInvoice invoice)
        {
            List<ItemDetails> itemList = new List<ItemDetails>(invoice.InvoiceDetails);
            PdfPTable table = new PdfPTable(7);
            table.TotalWidth = 120f;
            float[] widths = new float[] { 0.5f, 4f, 1f, 1f, 0.5f, 1f, 2f };
            table.SetWidths(widths);

            //Add Headers
            table.AddCell(HeaderCell("#"));
            table.AddCell(HeaderCell("Description"));
            table.AddCell(HeaderCell("Price"));
            table.AddCell(HeaderCell("VAT"));
            table.AddCell(HeaderCell("Qty"));
            table.AddCell(HeaderCell("Weight"));
            table.AddCell(HeaderCell("Item Total"));

            //Add the data from individual items
            foreach (var item in itemList)
            {
                table.AddCell(QuantityCell((itemList.IndexOf(item) + 1).ToString() + "."));
                table.AddCell(item.Item);
                table.AddCell(PriceCell(item.Total.ToString("C")));
                table.AddCell(PriceCell(item.VATAmount.ToString("C")));
                table.AddCell(QuantityCell(item.Quantity.ToString()));
                table.AddCell(QuantityCell(item.Weight.ToString()));
                table.AddCell(PriceCell(item.TotalPlusVAT.ToString("C")));
            }

            // Net Total Cell
            PdfPCell netTotal = new PdfPCell(new Phrase(invoice.NetTotal.ToString("C")));
            netTotal.Border = Rectangle.TOP_BORDER;
            netTotal.HorizontalAlignment = 2;

            PdfPCell ntText = new PdfPCell(new Phrase("Net Total:"));
            ntText.HorizontalAlignment = 2;
            ntText.Colspan = 4;
            ntText.Border = Rectangle.TOP_BORDER;
            table.AddCell(BlankCell());
            table.AddCell(BlankCell());
            table.AddCell(ntText);
            table.AddCell(netTotal);

            //Total Vat Cell
            PdfPCell vatCell = new PdfPCell(new Phrase(invoice.TotalVATAmount.ToString("C")));
            vatCell.Border = Rectangle.TOP_BORDER;
            vatCell.HorizontalAlignment = 2;

            PdfPCell vatText = new PdfPCell(new Phrase("Total VAT:"));
            vatText.HorizontalAlignment = 2;
            vatText.Colspan = 4;
            vatText.Border = Rectangle.TOP_BORDER;

            table.AddCell(BlankCell());
            table.AddCell(BlankCell());
            table.AddCell(vatText);
            table.AddCell(vatCell);

            // Sub Total Cell
            PdfPCell subtotal = new PdfPCell(new Phrase(invoice.SubTotal.ToString("C")));
            subtotal.Border = Rectangle.TOP_BORDER;
            subtotal.HorizontalAlignment = 2;

            PdfPCell stText = new PdfPCell(new Phrase("Subtotal:"));
            stText.HorizontalAlignment = 2;
            stText.Colspan = 4;
            stText.Border = Rectangle.TOP_BORDER;
            table.AddCell(BlankCell());
            table.AddCell(BlankCell());
            table.AddCell(stText);
            table.AddCell(subtotal);

            // Service Charge Cell
            PdfPCell serviceCharge = new PdfPCell(new Phrase(invoice.ServiceCharge.ToString("C")));
            serviceCharge.Border = Rectangle.TOP_BORDER;
            serviceCharge.HorizontalAlignment = 2;

            PdfPCell scText = new PdfPCell(new Phrase("Service Charge:"));
            scText.HorizontalAlignment = 2;
            scText.Colspan = 4;
            scText.Border = Rectangle.TOP_BORDER;
            table.AddCell(BlankCell());
            table.AddCell(BlankCell());
            table.AddCell(scText);
            table.AddCell(serviceCharge);

            // Grand Total Cell
            PdfPCell grandTotal = new PdfPCell(new Phrase(invoice.TotalToPay.ToString("C")));
            grandTotal.Border = Rectangle.TOP_BORDER;
            grandTotal.HorizontalAlignment = 2;

            PdfPCell gtText = new PdfPCell(new Phrase("Grand Total:"));
            gtText.HorizontalAlignment = 2;
            gtText.Colspan = 4;
            gtText.Border = Rectangle.TOP_BORDER;

            table.AddCell(BlankCell());
            table.AddCell(BlankCell());
            table.AddCell(gtText);
            table.AddCell(grandTotal);

            return table;
        }

        private PdfPCell BlankCell()
        {
            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.TOP_BORDER;

            return cell;
        }

        private PdfPCell PriceCell(string price)
        {
            PdfPCell cell = new PdfPCell();
            cell.HorizontalAlignment = 2;
            cell.Phrase = new Phrase(price);
            return cell;
        }

        private PdfPCell QuantityCell(string quantity)
        {
            PdfPCell cell = new PdfPCell();
            cell.HorizontalAlignment = 1;
            cell.Phrase = new Phrase(quantity);
            return cell;
        }

        private PdfPCell HeaderCell(string CellContent)
        {
            PdfPHeaderCell cell = new PdfPHeaderCell();
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            cell.Phrase = new Phrase(CellContent);
            return cell;
        }
    }
}