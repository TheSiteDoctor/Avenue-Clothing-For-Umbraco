﻿using System.Linq;
using System.Web.Mvc;
using UCommerce;
using Ucommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using AvenueClothing.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace AvenueClothing.Controllers
{
    public class BillingController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [HttpGet]
        public override ActionResult Index(ContentModel model)
        {
            var addressDetails = new AddressDetailsViewModel();

            var shippingInformation = TransactionLibrary.GetShippingInformation();
            var billingInformation = TransactionLibrary.GetBillingInformation();

            addressDetails.BillingAddress.FirstName = billingInformation.FirstName;
            addressDetails.BillingAddress.LastName = billingInformation.LastName;
            addressDetails.BillingAddress.EmailAddress = billingInformation.EmailAddress;
            addressDetails.BillingAddress.PhoneNumber = billingInformation.PhoneNumber;
            addressDetails.BillingAddress.MobilePhoneNumber = billingInformation.MobilePhoneNumber;
            addressDetails.BillingAddress.Line1 = billingInformation.Line1;
            addressDetails.BillingAddress.Line2 = billingInformation.Line2;
            addressDetails.BillingAddress.PostalCode = billingInformation.PostalCode;
            addressDetails.BillingAddress.City = billingInformation.City;
            addressDetails.BillingAddress.State = billingInformation.State;
            addressDetails.BillingAddress.Attention = billingInformation.Attention;
            addressDetails.BillingAddress.CompanyName = billingInformation.CompanyName;
            addressDetails.BillingAddress.CountryId =
                billingInformation.Country != null ? billingInformation.Country.CountryId : -1;

            addressDetails.ShippingAddress.FirstName = shippingInformation.FirstName;
            addressDetails.ShippingAddress.LastName = shippingInformation.LastName;
            addressDetails.ShippingAddress.EmailAddress = shippingInformation.EmailAddress;
            addressDetails.ShippingAddress.PhoneNumber = shippingInformation.PhoneNumber;
            addressDetails.ShippingAddress.MobilePhoneNumber = shippingInformation.MobilePhoneNumber;
            addressDetails.ShippingAddress.Line1 = shippingInformation.Line1;
            addressDetails.ShippingAddress.Line2 = shippingInformation.Line2;
            addressDetails.ShippingAddress.PostalCode = shippingInformation.PostalCode;
            addressDetails.ShippingAddress.City = shippingInformation.City;
            addressDetails.ShippingAddress.State = shippingInformation.State;
            addressDetails.ShippingAddress.Attention = shippingInformation.Attention;
            addressDetails.ShippingAddress.CompanyName = shippingInformation.CompanyName;
            addressDetails.ShippingAddress.CountryId =
                shippingInformation.Country != null ? shippingInformation.Country.CountryId : -1;

            addressDetails.AvailableCountries = Country.All().ToList()
                .Select(x => new SelectListItem() {Text = x.Name, Value = x.CountryId.ToString()}).ToList();

            return View("/Views/BillingShippingAddress.cshtml", addressDetails);
        }

        [HttpPost]
        public ActionResult Index(AddressDetailsViewModel addressDetails)
        {
            if (addressDetails.IsShippingAddressDifferent)
            {
                EditBillingInformation(addressDetails.BillingAddress);
                EditShippingInformation(addressDetails.ShippingAddress);
            }

            else
            {
                EditBillingInformation(addressDetails.BillingAddress);
                EditShippingInformation(addressDetails.BillingAddress);
            }

            TransactionLibrary.ExecuteBasketPipeline();

            var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");
            var shipping = parent.Children(x => x.Name == "Shipping").FirstOrDefault();
            return Redirect(shipping.Url);
        }

        private void EditShippingInformation(AddressViewModel shippingAddress)
        {
            TransactionLibrary.EditShipmentInformation(
                UCommerce.Constants.DefaultShipmentAddressName,
                shippingAddress.FirstName,
                shippingAddress.LastName,
                shippingAddress.EmailAddress,
                shippingAddress.PhoneNumber,
                shippingAddress.MobilePhoneNumber,
                shippingAddress.CompanyName,
                shippingAddress.Line1,
                shippingAddress.Line2,
                shippingAddress.PostalCode,
                shippingAddress.City,
                shippingAddress.State,
                shippingAddress.Attention,
                shippingAddress.CountryId);
        }

        private void EditBillingInformation(AddressViewModel billingAddress)
        {
            TransactionLibrary.EditBillingInformation(
                billingAddress.FirstName,
                billingAddress.LastName,
                billingAddress.EmailAddress,
                billingAddress.PhoneNumber,
                billingAddress.MobilePhoneNumber,
                billingAddress.CompanyName,
                billingAddress.Line1,
                billingAddress.Line2,
                billingAddress.PostalCode,
                billingAddress.City,
                billingAddress.State,
                billingAddress.Attention,
                billingAddress.CountryId);
        }
    }
}