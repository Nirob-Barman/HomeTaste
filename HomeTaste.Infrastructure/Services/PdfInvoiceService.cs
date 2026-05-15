using HomeTaste.Application.DTOs.Order;
using HomeTaste.Application.Interfaces.Order;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HomeTaste.Infrastructure.Services
{
    public class PdfInvoiceService : IPdfInvoiceService
    {
        public byte[] Generate(OrderResponse order)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(col =>
                    {
                        // Header
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item()
                                    .DefaultTextStyle(s => s.Bold().FontSize(22).FontColor("#f97316"))
                                    .Text("HomeTaste");
                                c.Item()
                                    .DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Darken1))
                                    .Text("Homemade Food Delivery");
                            });

                            row.ConstantItem(160).Column(c =>
                            {
                                c.Item()
                                    .AlignRight()
                                    .DefaultTextStyle(s => s.Bold().FontSize(18).FontColor(Colors.Grey.Darken3))
                                    .Text("INVOICE");
                                c.Item()
                                    .AlignRight()
                                    .DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Darken1))
                                    .Text($"#{order.Id.ToString()[..8].ToUpper()}");
                                c.Item()
                                    .AlignRight()
                                    .DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Darken1))
                                    .Text(order.CreatedAt?.ToString("MMMM d, yyyy") ?? string.Empty);
                            });
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Meta info
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().DefaultTextStyle(s => s.FontSize(8).FontColor(Colors.Grey.Medium)).Text("STATUS");
                                c.Item().Text(order.StatusLabel ?? order.Status.ToString());
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().DefaultTextStyle(s => s.FontSize(8).FontColor(Colors.Grey.Medium)).Text("DATE");
                                c.Item().Text(order.CreatedAt?.ToString("MMM d, yyyy HH:mm") ?? "—");
                            });
                            if (!string.IsNullOrEmpty(order.AddressSummary))
                            {
                                row.RelativeItem(2).Column(c =>
                                {
                                    c.Item().DefaultTextStyle(s => s.FontSize(8).FontColor(Colors.Grey.Medium)).Text("DELIVER TO");
                                    c.Item().Text(order.AddressSummary);
                                });
                            }
                        });

                        col.Item().PaddingVertical(14).Element(c => ItemsTable(c, order));

                        // Totals
                        col.Item().AlignRight().Width(220).Column(totals =>
                        {
                            TotalRow(totals, "Subtotal", $"${order.SubTotal:F2}");

                            if (order.DeliveryFee > 0)
                                TotalRow(totals, "Delivery", $"${order.DeliveryFee:F2}");
                            else
                                TotalRow(totals, "Delivery", "Free", "#16a34a");

                            if (order.DiscountAmount > 0)
                                TotalRow(totals, "Discount", $"−${order.DiscountAmount:F2}", "#16a34a");

                            if (order.TaxAmount > 0)
                                TotalRow(totals, "Tax", $"${order.TaxAmount:F2}");

                            totals.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            totals.Item().PaddingTop(6).Row(row =>
                            {
                                row.RelativeItem()
                                    .DefaultTextStyle(s => s.Bold().FontSize(11))
                                    .Text("Total");
                                row.AutoItem()
                                    .AlignRight()
                                    .DefaultTextStyle(s => s.Bold().FontSize(12).FontColor("#f97316"))
                                    .Text($"${order.TotalAmount:F2}");
                            });
                        });

                        if (!string.IsNullOrEmpty(order.Notes))
                        {
                            col.Item().PaddingTop(16).Column(c =>
                            {
                                c.Item().DefaultTextStyle(s => s.FontSize(8).FontColor(Colors.Grey.Medium)).Text("ORDER NOTES");
                                c.Item().DefaultTextStyle(s => s.FontColor(Colors.Grey.Darken2)).Text(order.Notes);
                            });
                        }

                        col.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(8)
                            .AlignCenter()
                            .DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Medium))
                            .Text("Thank you for choosing HomeTaste!");
                    });
                });
            }).GeneratePdf();
        }

        private static void ItemsTable(IContainer container, OrderResponse order)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#f97316").Padding(7)
                        .DefaultTextStyle(s => s.Bold().FontSize(9).FontColor(Colors.White))
                        .Text("Item");
                    header.Cell().Background("#f97316").Padding(7).AlignCenter()
                        .DefaultTextStyle(s => s.Bold().FontSize(9).FontColor(Colors.White))
                        .Text("Qty");
                    header.Cell().Background("#f97316").Padding(7).AlignRight()
                        .DefaultTextStyle(s => s.Bold().FontSize(9).FontColor(Colors.White))
                        .Text("Unit Price");
                    header.Cell().Background("#f97316").Padding(7).AlignRight()
                        .DefaultTextStyle(s => s.Bold().FontSize(9).FontColor(Colors.White))
                        .Text("Total");
                });

                var items = order.Items ?? [];
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    string bg = i % 2 == 0 ? "#FFFFFF" : "#f9fafb";

                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#f3f4f6").Padding(7)
                        .Text(item.MealName ?? "—");
                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#f3f4f6").Padding(7).AlignCenter()
                        .Text(item.Quantity.ToString());
                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#f3f4f6").Padding(7).AlignRight()
                        .Text($"${item.UnitPrice:F2}");
                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#f3f4f6").Padding(7).AlignRight()
                        .Text($"${item.TotalPrice:F2}");
                }
            });
        }

        private static void TotalRow(ColumnDescriptor column, string label, string value, string valueColor = "#374151")
        {
            column.Item().PaddingVertical(2).Row(row =>
            {
                row.RelativeItem()
                    .DefaultTextStyle(s => s.FontSize(9).FontColor(Colors.Grey.Darken1))
                    .Text(label);
                row.AutoItem()
                    .AlignRight()
                    .DefaultTextStyle(s => s.FontSize(9).FontColor(valueColor))
                    .Text(value);
            });
        }
    }
}
