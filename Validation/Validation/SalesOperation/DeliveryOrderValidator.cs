﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Interface.Validation;
using Core.DomainModel;
using Core.Interface.Service;
using Core.Constants;

namespace Validation.Validation
{
    public class DeliveryOrderValidator : IDeliveryOrderValidator
    {
        public DeliveryOrder VHasWarehouse(DeliveryOrder deliveryOrder, IWarehouseService _warehouseService)
        {
            Warehouse warehouse = _warehouseService.GetObjectById(deliveryOrder.WarehouseId);
            if (warehouse == null)
            {
                deliveryOrder.Errors.Add("WarehouseId", "Tidak terasosiasi dengan warehouse");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VHasSalesOrder(DeliveryOrder deliveryOrder, ISalesOrderService _salesOrderService)
        {
            SalesOrder salesOrder = _salesOrderService.GetObjectById(deliveryOrder.SalesOrderId);
            if (salesOrder == null)
            {
                deliveryOrder.Errors.Add("SalesOrderId", "Tidak terasosiasi dengan sales order");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VHasDeliveryDate(DeliveryOrder deliveryOrder)
        {
            if (deliveryOrder.DeliveryDate == null)
            {
                deliveryOrder.Errors.Add("DeliveryDate", "Tidak boleh kosong");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VHasBeenConfirmed(DeliveryOrder deliveryOrder)
        {
            if (!deliveryOrder.IsConfirmed)
            {
                deliveryOrder.Errors.Add("Generic", "Belum dikonfirmasi");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VHasNotBeenConfirmed(DeliveryOrder deliveryOrder)
        {
            if (deliveryOrder.IsConfirmed)
            {
                deliveryOrder.Errors.Add("Generic", "Tidak boleh sudah dikonfirmasi");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VHasDeliveryOrderDetails(DeliveryOrder deliveryOrder, IDeliveryOrderDetailService _deliveryOrderDetailService)
        {
            IList<DeliveryOrderDetail> details = _deliveryOrderDetailService.GetObjectsByDeliveryOrderId(deliveryOrder.Id);
            if (!details.Any())
            {
                deliveryOrder.Errors.Add("Generic", "Tidak memiliki delivery order detail");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VHasNoDeliveryOrderDetail(DeliveryOrder deliveryOrder, IDeliveryOrderDetailService _deliveryOrderDetailService)
        {
            IList<DeliveryOrderDetail> details = _deliveryOrderDetailService.GetObjectsByDeliveryOrderId(deliveryOrder.Id);
            if (details.Any())
            {
                deliveryOrder.Errors.Add("Generic", "Masih memiliki delivery order detail");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VSalesOrderHasBeenConfirmed(DeliveryOrder deliveryOrder, ISalesOrderService _salesOrderService)
        {
            SalesOrder salesOrder = _salesOrderService.GetObjectById(deliveryOrder.SalesOrderId);
            if (!salesOrder.IsConfirmed)
            {
                deliveryOrder.Errors.Add("Generic", "Sales order belum terkonfirmasi");
            }
            return deliveryOrder;

        }

        public DeliveryOrder VHasConfirmationDate(DeliveryOrder obj)
        {
            if (obj.ConfirmationDate == null)
            {
                obj.Errors.Add("ConfirmationDate", "Tidak boleh kosong");
            }
            return obj;
        }

        public DeliveryOrder VHasNoSalesInvoice(DeliveryOrder deliveryOrder, ISalesInvoiceService _salesInvoiceService)
        {
            IList<SalesInvoice> salesInvoices = _salesInvoiceService.GetObjectsByDeliveryOrderId(deliveryOrder.Id);
            if (salesInvoices.Any())
            {
                deliveryOrder.Errors.Add("Generic", "Sudah memiliki sales invoice");
            }
            return deliveryOrder;
        }

        public DeliveryOrder VCreateObject(DeliveryOrder deliveryOrder, ISalesOrderService _salesOrderService, IWarehouseService _warehouseService)
        {
            VHasWarehouse(deliveryOrder, _warehouseService);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasSalesOrder(deliveryOrder, _salesOrderService);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VSalesOrderHasBeenConfirmed(deliveryOrder, _salesOrderService);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasDeliveryDate(deliveryOrder);
            return deliveryOrder;
        }

        public DeliveryOrder VUpdateObject(DeliveryOrder deliveryOrder, ISalesOrderService _salesOrderService, IWarehouseService _warehouseService)
        {
            VCreateObject(deliveryOrder, _salesOrderService, _warehouseService);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasNotBeenConfirmed(deliveryOrder);
            return deliveryOrder;
        }

        public DeliveryOrder VDeleteObject(DeliveryOrder deliveryOrder, IDeliveryOrderDetailService _deliveryOrderDetailService)
        {
            VHasNotBeenConfirmed(deliveryOrder);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasNoDeliveryOrderDetail(deliveryOrder, _deliveryOrderDetailService);
            return deliveryOrder;
        }

        public DeliveryOrder VConfirmObject(DeliveryOrder deliveryOrder, IDeliveryOrderDetailService _deliveryOrderDetailService)
        {
            VHasConfirmationDate(deliveryOrder);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasNotBeenConfirmed(deliveryOrder);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasDeliveryOrderDetails(deliveryOrder, _deliveryOrderDetailService);
            return deliveryOrder;
        }

        public DeliveryOrder VUnconfirmObject(DeliveryOrder deliveryOrder, ISalesInvoiceService _salesInvoiceService)
        {
            VHasBeenConfirmed(deliveryOrder);
            if (!isValid(deliveryOrder)) { return deliveryOrder; }
            VHasNoSalesInvoice(deliveryOrder, _salesInvoiceService);
            return deliveryOrder;
        }

        public bool ValidCreateObject(DeliveryOrder deliveryOrder, ISalesOrderService _salesOrderService, IWarehouseService _warehouseService)
        {
            VCreateObject(deliveryOrder, _salesOrderService, _warehouseService);
            return isValid(deliveryOrder);
        }

        public bool ValidUpdateObject(DeliveryOrder deliveryOrder, ISalesOrderService _salesOrderService, IWarehouseService _warehouseService)
        {
            deliveryOrder.Errors.Clear();
            VUpdateObject(deliveryOrder, _salesOrderService, _warehouseService);
            return isValid(deliveryOrder);
        }

        public bool ValidDeleteObject(DeliveryOrder deliveryOrder, IDeliveryOrderDetailService _deliveryOrderDetailService)
        {
            deliveryOrder.Errors.Clear();
            VDeleteObject(deliveryOrder, _deliveryOrderDetailService);
            return isValid(deliveryOrder);
        }

        public bool ValidConfirmObject(DeliveryOrder deliveryOrder, IDeliveryOrderDetailService _deliveryOrderDetailService)
        {
            deliveryOrder.Errors.Clear();
            VConfirmObject(deliveryOrder, _deliveryOrderDetailService);
            return isValid(deliveryOrder);
        }

        public bool ValidUnconfirmObject(DeliveryOrder deliveryOrder, ISalesInvoiceService _salesInvoiceService)
        {
            deliveryOrder.Errors.Clear();
            VUnconfirmObject(deliveryOrder, _salesInvoiceService);
            return isValid(deliveryOrder);
        }

        public bool isValid(DeliveryOrder obj)
        {
            bool isValid = !obj.Errors.Any();
            return isValid;
        }

        public string PrintError(DeliveryOrder obj)
        {
            string erroroutput = "";
            KeyValuePair<string, string> first = obj.Errors.ElementAt(0);
            erroroutput += first.Key + "," + first.Value;
            foreach (KeyValuePair<string, string> pair in obj.Errors.Skip(1))
            {
                erroroutput += Environment.NewLine;
                erroroutput += pair.Key + "," + pair.Value;
            }
            return erroroutput;
        }

    }
}