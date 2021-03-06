﻿using System.Linq;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class OrganizationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrganizationEditModelValidator _organizationValidator;

        public OrganizationController(IMediator mediator, IOrganizationEditModelValidator validator)
        {
            _mediator = mediator;
            _organizationValidator = validator;
        }

        // GET: Organization
        public IActionResult Index()
        {
            var list = _mediator.Send(new OrganizationListQuery());
            return View(list);
        }

        // GET: Organization/Details/5
        public IActionResult Details(int id)
        {
            var organization = _mediator.Send(new OrganizationDetailQuery { Id = id });
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        // GET: Organization/Create
        public IActionResult Create()
        {
            return View("Edit");
        }

        // GET: Organization/Edit/5
        public IActionResult Edit(int id)
        {
            var organization = _mediator.Send(new OrganizationEditQuery { Id = id });
            if (organization == null)
            {
                return NotFound();
            }

            return View("Edit",organization);
        }

        // POST: Organization/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrganizationEditModel organization)
        {
            if (organization == null)
            {
                return BadRequest();
            }

            var errors = _organizationValidator.Validate(organization);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                bool isNameUnique = _mediator.Send(new OrganizationNameUniqueQuery() { OrganizationName = organization.Name, OrganizationId = organization.Id });
                if (isNameUnique)
                {
                    int id = _mediator.Send(new OrganizationEditCommand { Organization = organization });
                    return RedirectToAction("Details", new { id = id, area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError(nameof(organization.Name), "Organization with same name already exists. Please use different name.");
                }
            }

            return View("Edit", organization);
        }

        // GET: Organization/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            // Needs comments:  This method doesn't delete things.
            if (id == null)
            {
                return NotFound();
            }
            var organization = _mediator.Send(new OrganizationDetailQuery { Id = id.Value });
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _mediator.Send(new OrganizationDeleteCommand { Id= id });
            return RedirectToAction("Index");
        }               
    }
}
