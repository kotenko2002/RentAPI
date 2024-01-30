using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Service.Services.Cities.Views;
using Rent.Service.Services.Comments.Views;
using Rent.Service.Services.Properties.Views;
using Rent.Service.Services.Responses.Views;
using System.Diagnostics.CodeAnalysis;

namespace Rent.Tests.Infrastructure
{
    internal class UserEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals([AllowNull] User x, [AllowNull] User y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && string.Equals(x.Id, y.Id)
                && string.Equals(x.UserName, y.UserName);
        }

        public int GetHashCode([DisallowNull] User obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class CityEqualityComparer : IEqualityComparer<City>
    {
        public bool Equals([AllowNull] City x, [AllowNull] City y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && string.Equals(x.Name, y.Name);
        }

        public int GetHashCode([DisallowNull] City obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class CityViewEqualityComparer : IEqualityComparer<CityView>
    {
        public bool Equals([AllowNull] CityView x, [AllowNull] CityView y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && string.Equals(x.Name, y.Name);
        }

        public int GetHashCode([DisallowNull] CityView obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class PropertyEqualityComparer : IEqualityComparer<Property>
    {
        public bool Equals([AllowNull] Property x, [AllowNull] Property y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.LandlordId == y.LandlordId
                && x.CityId == y.CityId
                && x.Address == y.Address
                && x.Description == y.Description
                && x.Price == y.Price
                && x.Status == y.Status;
        }

        public int GetHashCode([DisallowNull] Property obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class PropertyViewEqualityComparer : IEqualityComparer<PropertyView>
    {
        public bool Equals([AllowNull] PropertyView x, [AllowNull] PropertyView y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && string.Equals(x.CityName, y.CityName)
                && string.Equals(x.Address, y.Address)
                && x.Price == y.Price
                && string.Equals(x.PhotoUrl, y.PhotoUrl);
        }

        public int GetHashCode([DisallowNull] PropertyView obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class PropertyDetailViewEqualityComparer : IEqualityComparer<PropertyDetailView>
    {
        public bool Equals([AllowNull] PropertyDetailView x, [AllowNull] PropertyDetailView y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.CityId == y.CityId
                && string.Equals(x.CityName, y.CityName)
                && string.Equals(x.Address, y.Address)
                && string.Equals(x.Description, y.Description)
                && x.Price == y.Price
                && x.Photos.SequenceEqual(y.Photos, new PhotoViewEqualityComparer());
        }

        public int GetHashCode([DisallowNull] PropertyDetailView obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class PhotoViewEqualityComparer : IEqualityComparer<PhotoView>
    {
        public bool Equals([AllowNull] PhotoView x, [AllowNull] PhotoView y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return string.Equals(x.Id, y.Id)
                && string.Equals(x.Url, y.Url);
        }

        public int GetHashCode([DisallowNull] PhotoView obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class CommentEqualityComparer : IEqualityComparer<Comment>
    {
        public bool Equals([AllowNull] Comment x, [AllowNull] Comment y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.TenantId == y.TenantId
                && x.PropertyId == y.PropertyId
                && x.Message == y.Message
                && x.Rate == y.Rate;
        }

        public int GetHashCode([DisallowNull] Comment obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class CommentViewEqualityComparer : IEqualityComparer<CommentView>
    {
        public bool Equals([AllowNull] CommentView x, [AllowNull] CommentView y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.UserName == y.UserName
                && x.Message == y.Message
                && x.Rate == y.Rate;
        }

        public int GetHashCode([DisallowNull] CommentView obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class PhotoEqualityComparer : IEqualityComparer<Photo>
    {
        public bool Equals([AllowNull] Photo x, [AllowNull] Photo y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.PropertyId == y.PropertyId;
        }

        public int GetHashCode([DisallowNull] Photo obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class ResponseEqualityComparer : IEqualityComparer<Response>
    {
        public bool Equals([AllowNull] Response x, [AllowNull] Response y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.TenantId == y.TenantId
                && x.PropertyId == y.PropertyId
                && x.Message == y.Message
                && x.Status == y.Status;
        }

        public int GetHashCode([DisallowNull] Response obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class ResponseViewEqualityComparer : IEqualityComparer<ResponseView>
    {
        public bool Equals([AllowNull] ResponseView x, [AllowNull] ResponseView y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.Email == y.Email
                && x.PhoneNumber == y.PhoneNumber
                && x.Message == y.Message
                && x.Status == y.Status;
        }

        public int GetHashCode([DisallowNull] ResponseView obj)
        {
            return obj.GetHashCode();
        }
    }
}
