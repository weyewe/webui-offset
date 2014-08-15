﻿using Core.DomainModel;
using Core.Interface.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Interface.Validation
{
    public interface IBarringOrderValidator
    {
        BarringOrder VHasUniqueCode(BarringOrder barringOrder, IBarringOrderService _barringOrderService);
        BarringOrder VHasBarringOrderDetails(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VHasQuantityReceived(BarringOrder barringOrder);
        BarringOrder VQuantityFinalAndRejectedIsLessThanOrEqualQuantityReceived(BarringOrder barringOrder);
        BarringOrder VQuantityReceivedEqualDetails(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VQuantityIsInStock(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService,
                                        IBarringService _barringService, IItemService _itemService, IWarehouseItemService _warehouseItemService);
        BarringOrder VHasBeenConfirmed(BarringOrder barringOrder);
        BarringOrder VHasNotBeenConfirmed(BarringOrder barringOrder);
        BarringOrder VHasBeenCompleted(BarringOrder barringOrder);
        BarringOrder VHasNotBeenCompleted(BarringOrder barringOrder);
        BarringOrder VAllDetailsHaveBeenFinishedOrRejected(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VAllDetailsHaveBeenCutOrRejected(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VAllDetailsHaveNotBeenCutNorRejected(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VCreateObject(BarringOrder barringOrder, IBarringOrderService _barringOrderService);
        BarringOrder VUpdateObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService, IBarringOrderService _barringOrderService);
        BarringOrder VDeleteObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VHasConfirmationDate(BarringOrder barringOrder);
        BarringOrder VConfirmObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService, IBarringService _barringService, IItemService _itemService, IWarehouseItemService _warehouseItemService);
        BarringOrder VUnconfirmObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VCompleteObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        BarringOrder VAdjustQuantity(BarringOrder barringOrder);

        bool ValidCreateObject(BarringOrder barringOrder, IBarringOrderService _barringOrderService);
        bool ValidUpdateObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService, IBarringOrderService _barringOrderService);
        bool ValidDeleteObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        bool ValidConfirmObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService, IBarringService _barringService, IItemService _itemService, IWarehouseItemService _warehouseItemService);
        bool ValidUnconfirmObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        bool ValidCompleteObject(BarringOrder barringOrder, IBarringOrderDetailService _barringOrderDetailService);
        bool ValidAdjustQuantity(BarringOrder barringOrder);
        bool isValid(BarringOrder barringOrder);
        string PrintError(BarringOrder barringOrder);
    }
}