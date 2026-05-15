using HomeTaste.Application.DTOs.Order;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Helpers.Email
{
    public static class OrderEmailTemplates
    {
        private static string OrderRef(Guid id) => id.ToString()[..8].ToUpperInvariant();

        public static string OrderConfirmation(OrderResponse order) =>
            Wrap($"Order Confirmed — #{OrderRef(order.Id)}", $"""
                <h2 style="color:#ea580c;margin:0 0 4px">Order Placed Successfully!</h2>
                <p style="color:#6b7280;margin:0 0 24px">
                    Hi there, your order <strong>#{OrderRef(order.Id)}</strong> has been received
                    and is awaiting confirmation.
                </p>

                {ItemsTable(order)}
                {Totals(order)}

                <table width="100%" cellpadding="0" cellspacing="0" style="margin:24px 0">
                    <tr>
                        <td style="background:#fff7ed;border-radius:8px;padding:16px">
                            <p style="margin:0;font-size:13px;color:#6b7280">
                                <strong style="color:#374151">Delivery to:</strong><br/>
                                {order.AddressSummary}
                            </p>
                            {(order.EstimatedDeliveryAt.HasValue
                                ? $"<p style=\"margin:8px 0 0;font-size:13px;color:#6b7280\"><strong style=\"color:#374151\">Estimated delivery:</strong><br/>{order.EstimatedDeliveryAt.Value:dddd, MMMM d 'at' h:mm tt}</p>"
                                : "")}
                            {(!string.IsNullOrWhiteSpace(order.Notes)
                                ? $"<p style=\"margin:8px 0 0;font-size:13px;color:#6b7280\"><strong style=\"color:#374151\">Notes:</strong><br/>{order.Notes}</p>"
                                : "")}
                        </td>
                    </tr>
                </table>

                <p style="font-size:13px;color:#9ca3af;margin:0">
                    We'll send you another email as your order progresses.
                </p>
            """);

        public static string StatusChanged(Guid orderId, OrderStatus status) =>
            Wrap($"Order Update — #{OrderRef(orderId)}", $"""
                <h2 style="color:#ea580c;margin:0 0 4px">{StatusHeadline(status)}</h2>
                <p style="color:#6b7280;margin:0 0 24px">
                    Your order <strong>#{OrderRef(orderId)}</strong> {StatusMessage(status)}
                </p>
                <div style="background:{StatusColor(status)};border-radius:24px;display:inline-block;padding:6px 16px;margin-bottom:24px">
                    <span style="font-size:13px;font-weight:600;color:#fff">{status}</span>
                </div>
                <p style="font-size:13px;color:#9ca3af;margin:0">
                    Questions? Reply to this email or contact our support team.
                </p>
            """);

        public static string OrderCancelled(Guid orderId, string? reason) =>
            Wrap($"Order Cancelled — #{OrderRef(orderId)}", $"""
                <h2 style="color:#ef4444;margin:0 0 4px">Order Cancelled</h2>
                <p style="color:#6b7280;margin:0 0 24px">
                    Your order <strong>#{OrderRef(orderId)}</strong> has been cancelled.
                </p>
                {(!string.IsNullOrWhiteSpace(reason)
                    ? $"<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"margin-bottom:24px\"><tr><td style=\"background:#fef2f2;border-radius:8px;padding:16px\"><p style=\"margin:0;font-size:13px;color:#991b1b\"><strong>Reason:</strong> {reason}</p></td></tr></table>"
                    : "")}
                <p style="font-size:13px;color:#9ca3af;margin:0">
                    If you were charged, a refund will be processed within 3–5 business days.
                    Questions? Contact our support team.
                </p>
            """);

        // ── Helpers ────────────────────────────────────────────────────────────

        private static string ItemsTable(OrderResponse order)
        {
            if (order.Items == null || order.Items.Count == 0) return "";

            var rows = string.Join("", order.Items.Select(i => $"""
                <tr>
                    <td style="padding:10px 0;border-bottom:1px solid #f3f4f6;font-size:14px;color:#374151">
                        {i.MealName} × {i.Quantity}
                        {(!string.IsNullOrWhiteSpace(i.SpecialInstructions)
                            ? $"<br/><span style=\"font-size:12px;color:#9ca3af\">{i.SpecialInstructions}</span>"
                            : "")}
                    </td>
                    <td style="padding:10px 0;border-bottom:1px solid #f3f4f6;font-size:14px;color:#374151;text-align:right">
                        ${i.TotalPrice:F2}
                    </td>
                </tr>
            """));

            return $"""
                <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:16px">
                    <thead>
                        <tr>
                            <th style="text-align:left;font-size:12px;color:#9ca3af;padding-bottom:8px;border-bottom:1px solid #e5e7eb">Item</th>
                            <th style="text-align:right;font-size:12px;color:#9ca3af;padding-bottom:8px;border-bottom:1px solid #e5e7eb">Price</th>
                        </tr>
                    </thead>
                    <tbody>{rows}</tbody>
                </table>
            """;
        }

        private static string Totals(OrderResponse order)
        {
            var rows = new System.Text.StringBuilder();
            rows.Append(TotalRow("Subtotal", order.SubTotal));
            if (order.LoyaltyDiscountAmount > 0)
                rows.Append(TotalRow($"Loyalty Points ({order.LoyaltyPointsUsed} pts)", -order.LoyaltyDiscountAmount, "#16a34a"));
            if (order.DiscountAmount > 0)
                rows.Append(TotalRow("Coupon Discount", -order.DiscountAmount, "#16a34a"));
            rows.Append(TotalRow("Tax (10%)", order.TaxAmount));
            rows.Append(TotalRow("Total", order.TotalAmount, "#ea580c", bold: true));

            return $"""
                <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:24px">
                    {rows}
                </table>
            """;
        }

        private static string TotalRow(string label, decimal amount, string color = "#374151", bool bold = false) =>
            $"""
            <tr>
                <td style="padding:4px 0;font-size:13px;color:#6b7280">{label}</td>
                <td style="padding:4px 0;font-size:13px;text-align:right;color:{color};{(bold ? "font-weight:700;font-size:15px" : "")}">
                    {(amount < 0 ? $"-${Math.Abs(amount):F2}" : $"${amount:F2}")}
                </td>
            </tr>
            """;

        private static string StatusHeadline(OrderStatus status) => status switch
        {
            OrderStatus.Confirmed       => "Order Confirmed!",
            OrderStatus.Preparing       => "Your Meal is Being Prepared",
            OrderStatus.ReadyForPickup  => "Order Ready for Pickup",
            OrderStatus.OutForDelivery  => "On Its Way!",
            OrderStatus.Delivered       => "Order Delivered!",
            _                           => $"Order {status}"
        };

        private static string StatusMessage(OrderStatus status) => status switch
        {
            OrderStatus.Confirmed       => "has been confirmed and will be prepared shortly.",
            OrderStatus.Preparing       => "is now being prepared by our kitchen.",
            OrderStatus.ReadyForPickup  => "is ready and waiting for a delivery driver.",
            OrderStatus.OutForDelivery  => "is out for delivery and will arrive soon.",
            OrderStatus.Delivered       => "has been delivered. Enjoy your meal!",
            _                           => $"status has been updated to {status}."
        };

        private static string StatusColor(OrderStatus status) => status switch
        {
            OrderStatus.Confirmed       => "#2563eb",
            OrderStatus.Preparing       => "#d97706",
            OrderStatus.ReadyForPickup  => "#7c3aed",
            OrderStatus.OutForDelivery  => "#0891b2",
            OrderStatus.Delivered       => "#16a34a",
            _                           => "#6b7280"
        };

        private static string Wrap(string title, string body) => $"""
            <!DOCTYPE html>
            <html>
            <body style="margin:0;padding:0;background:#f9fafb;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',sans-serif">
                <table width="100%" cellpadding="0" cellspacing="0" style="background:#f9fafb;padding:40px 16px">
                    <tr>
                        <td align="center">
                            <table width="100%" cellpadding="0" cellspacing="0" style="max-width:560px;background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 1px 3px rgba(0,0,0,.1)">
                                <!-- Header -->
                                <tr>
                                    <td style="background:#ea580c;padding:24px 32px">
                                        <h1 style="margin:0;font-size:20px;color:#ffffff;font-weight:700">HomeTaste</h1>
                                        <p style="margin:4px 0 0;font-size:13px;color:#fed7aa">{title}</p>
                                    </td>
                                </tr>
                                <!-- Body -->
                                <tr>
                                    <td style="padding:32px">
                                        {body}
                                    </td>
                                </tr>
                                <!-- Footer -->
                                <tr>
                                    <td style="background:#f9fafb;padding:16px 32px;border-top:1px solid #f3f4f6">
                                        <p style="margin:0;font-size:12px;color:#9ca3af;text-align:center">
                                            © {DateTime.UtcNow.Year} HomeTaste. All rights reserved.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
        """;
    }
}
