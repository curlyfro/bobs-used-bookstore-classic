﻿using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Areas.Admin.Models.ReferenceData;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class ReferenceDataController : AdminAreaControllerBase
    {
        private readonly IReferenceDataService referenceDataService;

        public ReferenceDataController(IReferenceDataService referenceDataService)
        {
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(ReferenceDataFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            var referenceDataItems = await referenceDataService.GetReferenceDataAsync(filters, pageIndex, pageSize);

            return View(new ReferenceDataIndexViewModel(referenceDataItems, filters));
        }

        public ActionResult Create(ReferenceDataType? selectedReferenceDataType = null)
        {
            var model = new ReferenceDataItemCreateUpdateViewModel();

            if (selectedReferenceDataType.HasValue) model.SelectedReferenceDataType = selectedReferenceDataType.Value;

            model.DataTypes = Enum.GetValues(typeof(ReferenceDataType)).Cast<ReferenceDataType>()
                .Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() });

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ReferenceDataItemCreateUpdateViewModel model)
        {
            var dto = new CreateReferenceDataItemDto(model.SelectedReferenceDataType, model.Text);

            await referenceDataService.CreateAsync(dto);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Update(int id)
        {
            var referenceDataItem = await referenceDataService.GetReferenceDataItemAsync(id);

            var model = new ReferenceDataItemCreateUpdateViewModel(referenceDataItem);

            model.DataTypes = Enum.GetValues(typeof(ReferenceDataType)).Cast<ReferenceDataType>()
                .Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() });

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(ReferenceDataItemCreateUpdateViewModel model)
        {
            var dto = new UpdateReferenceDataItemDto(model.Id, model.SelectedReferenceDataType, model.Text);

            await referenceDataService.UpdateAsync(dto);

            return RedirectToAction("Index");
        }
    }
}
