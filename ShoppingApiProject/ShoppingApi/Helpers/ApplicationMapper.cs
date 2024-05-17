using AutoMapper;
using ShoppingApi.OutputModel;
using ShoppingApi.Shipment;

namespace ShoppingApi.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() {
            CreateMap<ShipmentDetails, ShippingOut>().ReverseMap();
        }
    }
}
