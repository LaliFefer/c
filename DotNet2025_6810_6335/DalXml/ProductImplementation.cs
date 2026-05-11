using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DalApi;
using DO;
using System;

namespace Dal;

internal class ProductImplementation : IProduct
{
 private static readonly string s_file = global::DalXml.XmlHelper.Find(Path.Combine("DalXml", "products.xml"));

 private static List<Product> Load()
 {
     if (!File.Exists(s_file)) return new List<Product>();
     var ser = new XmlSerializer(typeof(List<Product>));
     using var fs = new FileStream(s_file, FileMode.Open);
     return (List<Product>?)ser.Deserialize(fs) ?? new List<Product>();
 }

 private static void Save(List<Product> list)
 {
 var ser = new XmlSerializer(typeof(List<Product>));
 using var fs = new FileStream(s_file, FileMode.Create);
 ser.Serialize(fs, list);
 }

 public int Create(Product item)
 {
 if (item is null) throw new ArgumentNullException(nameof(item));
 var list = Load();
 if (list.Any(p => p.IDNumber == item.IDNumber && item.IDNumber !=0))
 throw new DalFacade.DalExceptions.DalAlreadyExistsException($"Product with ID {item.IDNumber} already exists.");

 int newId = item.IDNumber ==0 ? global::DalXml.Config.ProductNum : item.IDNumber;
 var newItem = item with { IDNumber = newId };
 list.Add(newItem);
 Save(list);
 return newId;
 }

 public Product? Read(int id)
 {
 var list = Load();
 return list.FirstOrDefault(p => p.IDNumber == id);
 }

 public List<Product?> ReadAll(Func<Product, bool>? filter = null)
 {
 var list = Load();
 if (filter == null) return list.Select(p => (Product?)p).ToList();
 return list.Where(filter).Select(p => (Product?)p).ToList();
 }

 public Product? Read(Func<Product, bool> filter)
 {
 if (filter == null) return null;
 var list = Load();
 return list.FirstOrDefault(filter);
 }

 public void Update(Product item)
 {
 if (item is null) throw new ArgumentNullException(nameof(item));
 var list = Load();
 var existing = list.FirstOrDefault(p => p.IDNumber == item.IDNumber);
 if (existing is null) throw new DalFacade.DalExceptions.DalDoesNotExistException($"Product with ID {item.IDNumber} does not exist.");
 list = list.Select(p => p.IDNumber == item.IDNumber ? item : p).ToList();
 Save(list);
 }

 public void Delete(int id)
 {
 var list = Load();
 var existing = list.FirstOrDefault(p => p.IDNumber == id);
 if (existing is null) throw new DalFacade.DalExceptions.DalDoesNotExistException($"Product with ID {id} does not exist.");
 list = list.Where(p => p.IDNumber != id).ToList();
 Save(list);
 }
}
