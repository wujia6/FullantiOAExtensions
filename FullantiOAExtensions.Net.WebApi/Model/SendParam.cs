using FullantiOAExtensions.Core.Database.Entities;

namespace FullantiOAExtensions.Net.WebApi.Model
{
    public class SendParam
    {
        public string Account { get; set; }
        public List<Salary> Salaries { get; set; }
    }
}
