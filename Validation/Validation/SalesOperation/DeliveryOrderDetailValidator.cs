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
    public class DeliveryOrderDetailValidator : IDeliveryOrderDetailValidator
    {
        public DeliveryOrderDetail VHasDeliveryOrder(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderService _purchaseReceivalService)
        {
            DeliveryOrder deliveryOrder = _purchaseReceivalService.GetObjectById(deliveryOrderDetail.DeliveryOrderId);
            if (deliveryOrder == null)
            {
                deliveryOrderDetail.Errors.Add("DeliveryOrder", "Tidak boleh tidak ada");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VHasItem(DeliveryOrderDetail deliveryOrderDetail, IItemService _itemService)
        {
            Item item = _itemService.GetObjectById(deliveryOrderDetail.ItemId);
            if (item == null)
            {
                deliveryOrderDetail.Errors.Add("Item", "Tidak boleh tidak ada");
            }
            return deliveryOrderDetail;
        }


        public DeliveryOrderDetail VNonNegativeQuantity(DeliveryOrderDetail deliveryOrderDetail)
        {
            if (deliveryOrderDetail.Quantity <= 0)
            {
                deliveryOrderDetail.Errors.Add("Quantity", "Tidak boleh negatif");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VHasSalesOrderDetail(DeliveryOrderDetail deliveryOrderDetail, ISalesOrderDetailService _salesOrderDetailService)
        {
            SalesOrderDetail salesOrderDetail = _salesOrderDetailService.GetObjectById(deliveryOrderDetail.SalesOrderDetailId);
            if (salesOrderDetail == null)
            {
                deliveryOrderDetail.Errors.Add("SalesOrderDetail", "Tidak boleh tidak ada");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VUniqueSalesOrderDetail(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderDetailService _deliveryOrderDetailService, IItemService _itemService)
        {
            IList<DeliveryOrderDetail> details = _deliveryOrderDetailService.GetObjectsByDeliveryOrderId(deliveryOrderDetail.DeliveryOrderId);
            foreach (var detail in details)
            {
                if (detail.SalesOrderDetailId == deliveryOrderDetail.SalesOrderDetailId && detail.Id != deliveryOrderDetail.Id)
                {
                    deliveryOrderDetail.Errors.Add("SalesOrderDetail", "Tidak boleh memiliki lebih dari 2 Delivery Order Detail");
                    return deliveryOrderDetail;
                }
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VSalesOrderDetailHasBeenConfirmed(DeliveryOrderDetail deliveryOrderDetail, ISalesOrderDetailService _salesOrderDetailService)
        {
            SalesOrderDetail salesOrderDetail = _salesOrderDetailService.GetObjectById(deliveryOrderDetail.SalesOrderDetailId);
            if (!salesOrderDetail.IsConfirmed)
            {
                deliveryOrderDetail.Errors.Add("Generic", "Sales Order Detail belum dikonfirmasi");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VQuantityOfDeliveryOrderDetailsIsLessThanOrEqualSalesOrderDetail(DeliveryOrderDetail deliveryOrderDetail,
                                     IDeliveryOrderDetailService _deliveryOrderDetailService, ISalesOrderDetailService _salesOrderDetailService)
        {
            SalesOrderDetail salesOrderDetail = _salesOrderDetailService.GetObjectById(deliveryOrderDetail.SalesOrderDetailId);
            IList<DeliveryOrderDetail> details = _deliveryOrderDetailService.GetObjectsBySalesOrderDetailId(deliveryOrderDetail.SalesOrderDetailId);

            int totalReceivalQuantity = 0;
            foreach (var detail in details)
            {
                totalReceivalQuantity += detail.Quantity;
            }
            if (totalReceivalQuantity > salesOrderDetail.Quantity)
            {
                int maxquantity = totalReceivalQuantity - deliveryOrderDetail.Quantity;
                deliveryOrderDetail.Errors.Add("Generic", "Hanya boleh maksimum " + maxquantity);
            }
            return deliveryOrderDetail;
        }
        
        public DeliveryOrderDetail VDeliveryOrderAndSalesOrderDetailHaveTheSameSalesOrder(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderService _purchaseReceivalService, ISalesOrderDetailService _salesOrderDetailService)
        {
            DeliveryOrder deliveryOrder = _purchaseReceivalService.GetObjectById(deliveryOrderDetail.DeliveryOrderId);
            SalesOrderDetail salesOrderDetail = _salesOrderDetailService.GetObjectById(deliveryOrderDetail.SalesOrderDetailId);
            if (deliveryOrder.SalesOrderId != salesOrderDetail.SalesOrderId)
            {
                deliveryOrderDetail.Errors.Add("Generic", "Sales order dari sales order detail dan delivery order tidak sama");
                return deliveryOrderDetail;
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VHasItemQuantity(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderService _deliveryOrderService, IItemService _itemService, IWarehouseItemService _warehouseItemService)
        {
            DeliveryOrder deliveryOrder = _deliveryOrderService.GetObjectById(deliveryOrderDetail.DeliveryOrderId);
            WarehouseItem warehouseItem = _warehouseItemService.FindOrCreateObject(deliveryOrder.WarehouseId, deliveryOrderDetail.ItemId);
            Item item = _itemService.GetObjectById(deliveryOrderDetail.ItemId);
            if (item.Quantity - deliveryOrderDetail.Quantity < 0)
            {
                deliveryOrderDetail.Errors.Add("Generic", "Item quantity kurang dari quantity untuk di kirim");
            }
            else if (warehouseItem.Quantity - deliveryOrderDetail.Quantity < 0)
            {
                deliveryOrderDetail.Errors.Add("Generic", "WarehouseItem quantity kurang dari quantity untuk di kirim");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VHasBeenConfirmed(DeliveryOrderDetail deliveryOrderDetail)
        {
            if (!deliveryOrderDetail.IsConfirmed)
            {
                deliveryOrderDetail.Errors.Add("Generic", "Tidak boleh sudah dikonfirmasi.");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VHasNotBeenConfirmed(DeliveryOrderDetail deliveryOrderDetail)
        {
            if (deliveryOrderDetail.IsConfirmed)
            {
                deliveryOrderDetail.Errors.Add("Generic", "Tidak boleh sudah dikonfirmasi.");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VHasConfirmationDate(DeliveryOrderDetail obj)
        {
            if (obj.ConfirmationDate == null)
            {
                obj.Errors.Add("ConfirmationDate", "Tidak boleh kosong");
            }
            return obj;
        }

        public DeliveryOrderDetail VHasNoSalesInvoiceDetail(DeliveryOrderDetail deliveryOrderDetail, ISalesInvoiceDetailService _salesInvoiceDetailService)
        {
            IList<SalesInvoiceDetail> salesInvoiceDetails = _salesInvoiceDetailService.GetObjectsByDeliveryOrderDetailId(deliveryOrderDetail.Id);
            if (salesInvoiceDetails.Any())
            {
                deliveryOrderDetail.Errors.Add("Generic", "Sudah memiliki sales invoice detail");
            }
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VCreateObject(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderDetailService _deliveryOrderDetailService,
                                                 IDeliveryOrderService _deliveryOrderService, ISalesOrderDetailService _salesOrderDetailService, IItemService _itemService)
        {
            VHasDeliveryOrder(deliveryOrderDetail, _deliveryOrderService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VHasItem(deliveryOrderDetail, _itemService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VHasSalesOrderDetail(deliveryOrderDetail, _salesOrderDetailService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VSalesOrderDetailHasBeenConfirmed(deliveryOrderDetail, _salesOrderDetailService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VDeliveryOrderAndSalesOrderDetailHaveTheSameSalesOrder(deliveryOrderDetail, _deliveryOrderService, _salesOrderDetailService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VNonNegativeQuantity(deliveryOrderDetail);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VQuantityOfDeliveryOrderDetailsIsLessThanOrEqualSalesOrderDetail(deliveryOrderDetail, _deliveryOrderDetailService, _salesOrderDetailService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VUniqueSalesOrderDetail(deliveryOrderDetail, _deliveryOrderDetailService, _itemService);
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VUpdateObject(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderDetailService _deliveryOrderDetailService,
                                                 IDeliveryOrderService _deliveryOrderService, ISalesOrderDetailService _salesOrderDetailService, IItemService _itemService)
        {
            VHasNotBeenConfirmed(deliveryOrderDetail);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VCreateObject(deliveryOrderDetail, _deliveryOrderDetailService, _deliveryOrderService, _salesOrderDetailService, _itemService);
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VDeleteObject(DeliveryOrderDetail deliveryOrderDetail)
        {
            VHasNotBeenConfirmed(deliveryOrderDetail);
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VConfirmObject(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderService _deliveryOrderService,
                                                  IDeliveryOrderDetailService _deliveryOrderDetailService,  ISalesOrderDetailService _salesOrderDetailService,
                                                  IItemService _itemService, IWarehouseItemService _warehouseItemService)
        {
            VHasConfirmationDate(deliveryOrderDetail);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VHasNotBeenConfirmed(deliveryOrderDetail);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VHasItemQuantity(deliveryOrderDetail, _deliveryOrderService, _itemService, _warehouseItemService);
            if (!isValid(deliveryOrderDetail)) { return deliveryOrderDetail; }
            VQuantityOfDeliveryOrderDetailsIsLessThanOrEqualSalesOrderDetail(deliveryOrderDetail, _deliveryOrderDetailService, _salesOrderDetailService);
            return deliveryOrderDetail;
        }

        public DeliveryOrderDetail VUnconfirmObject(DeliveryOrderDetail deliveryOrderDetail, ISalesInvoiceDetailService _salesInvoiceDetailService)
        {
            VHasNoSalesInvoiceDetail(deliveryOrderDetail, _salesInvoiceDetailService);
            return deliveryOrderDetail;
        }

        public bool ValidCreateObject(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderDetailService _deliveryOrderDetailService,
                                      IDeliveryOrderService _deliveryOrderService, ISalesOrderDetailService _salesOrderDetailService, IItemService _itemService)
        {
            VCreateObject(deliveryOrderDetail, _deliveryOrderDetailService, _deliveryOrderService, _salesOrderDetailService, _itemService);
            return isValid(deliveryOrderDetail);
        }

        public bool ValidUpdateObject(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderDetailService _deliveryOrderDetailService,
                                      IDeliveryOrderService _deliveryOrderService, ISalesOrderDetailService _salesOrderDetailService, IItemService _itemService)
        {
            deliveryOrderDetail.Errors.Clear();
            VUpdateObject(deliveryOrderDetail, _deliveryOrderDetailService, _deliveryOrderService, _salesOrderDetailService, _itemService);
            return isValid(deliveryOrderDetail);
        }

        public bool ValidDeleteObject(DeliveryOrderDetail deliveryOrderDetail)
        {
            deliveryOrderDetail.Errors.Clear();
            VDeleteObject(deliveryOrderDetail);
            return isValid(deliveryOrderDetail);
        }

        public bool ValidConfirmObject(DeliveryOrderDetail deliveryOrderDetail, IDeliveryOrderService _deliveryOrderService,
                                       IDeliveryOrderDetailService _deliveryOrderDetailService, ISalesOrderDetailService _salesOrderDetailService,
                                       IItemService _itemService, IWarehouseItemService _warehouseItemService)
        {
            deliveryOrderDetail.Errors.Clear();
            VConfirmObject(deliveryOrderDetail, _deliveryOrderService, _deliveryOrderDetailService, _salesOrderDetailService, _itemService, _warehouseItemService);
            return isValid(deliveryOrderDetail);
        }

        public bool ValidUnconfirmObject(DeliveryOrderDetail deliveryOrderDetail, ISalesInvoiceDetailService _salesInvoiceDetailService) 
        {
            deliveryOrderDetail.Errors.Clear();
            VUnconfirmObject(deliveryOrderDetail, _salesInvoiceDetailService);
            return isValid(deliveryOrderDetail);
        }

        public bool isValid(DeliveryOrderDetail obj)
        {
            bool isValid = !obj.Errors.Any();
            return isValid;
        }

        public string PrintError(DeliveryOrderDetail obj)
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