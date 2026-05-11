using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DalApi;
using DO;
using System;

namespace Dal;

internal class CustomerImplementation : ICustomer
{
 private static readonly string s_file = global::DalXml.XmlHelper.Find(Path.Combine("DalXml", "customers.xml"));

 private static List<Customer> Load()
 {
     if (!File.Exists(s_file)) return new List<Customer>();
     var ser = new XmlSerializer(typeof(List<Customer>));
     using var fs = new FileStream(s_file, FileMode.Open);
     return (List<Customer>?)ser.Deserialize(fs) ?? new List<Customer>();
 }

 private static void Save(List<Customer> list)
 {
 var ser = new XmlSerializer(typeof(List<Customer>));
 using var fs = new FileStream(s_file, FileMode.Create);
 ser.Serialize(fs, list);
 }

 public int Create(Customer item)
 {
 if (item is null) throw new ArgumentNullException(nameof(item));
 var list = Load();
 if (list.Any(c => c.IDNumber == item.IDNumber))
 throw new DalFacade.DalExceptions.DalAlreadyExistsException($"Customer with ID {item.IDNumber} already exists.");
 list.Add(item);
 Save(list);
 return item.IDNumber;
 }

 public Customer? Read(int id)
 {
 var list = Load();
 return list.FirstOrDefault(c => c.IDNumber == id);
 }

 public List<Customer?> ReadAll(Func<Customer, bool>? filter = null)
 {
 var list = Load();
 if (filter == null) return list.Select(c => (Customer?)c).ToList();
 return list.Where(filter).Select(c => (Customer?)c).ToList();
 }

 public Customer? Read(Func<Customer, bool> filter)
 {
 if (filter == null) return null;
 var list = Load();
 return list.FirstOrDefault(filter);
 }

 public void Update(Customer item)
 {
 if (item is null) throw new ArgumentNullException(nameof(item));
 var list = Load();
 var existing = list.FirstOrDefault(c => c.IDNumber == item.IDNumber);
 if (existing is null) throw new DalFacade.DalExceptions.DalDoesNotExistException($"Customer with ID {item.IDNumber} does not exist.");
 list = list.Select(c => c.IDNumber == item.IDNumber ? item : c).ToList();
 Save(list);
 }

 public void Delete(int id)
 {
 var list = Load();
 var existing = list.FirstOrDefault(c => c.IDNumber == id);
 if (existing is null) throw new DalFacade.DalExceptions.DalDoesNotExistException($"Customer with ID {id} does not exist.");
 list = list.Where(c => c.IDNumber != id).ToList();
 Save(list);
 }
}
