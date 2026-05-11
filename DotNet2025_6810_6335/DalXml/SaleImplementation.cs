using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DalApi;
using DO;
using System;

namespace Dal;

internal class SaleImplementation : ISale
{
 private static readonly string s_file = global::DalXml.XmlHelper.Find(Path.Combine("DalXml", "sales.xml"));
 private static readonly string s_root = "ArrayOfSale";
 private static readonly string s_elem = "Sale";

 private static XElement LoadX()
 {
 if (!File.Exists(s_file)) return new XElement(s_root);
 return XElement.Load(s_file);
 }

 private static void SaveX(XElement root)
 {
 var doc = new XDocument(root);
 doc.Save(s_file);
 }

 public int Create(Sale item)
 {
 if (item is null) throw new ArgumentNullException(nameof(item));
 var root = LoadX();
 // check exists
 var exists = root.Elements(s_elem).FirstOrDefault(x => (int?)x.Element(nameof(Sale.IDNumber)) == item.IDNumber);
 if (exists != null && item.IDNumber !=0)
 throw new DalFacade.DalExceptions.DalAlreadyExistsException($"Sale with ID {item.IDNumber} already exists.");

 int newId = item.IDNumber ==0 ? global::DalXml.Config.SaleNum : item.IDNumber;
 var el = new XElement(s_elem,
 new XElement(nameof(Sale.IDNumber), newId),
 new XElement(nameof(Sale.ProductIDNumber), item.ProductIDNumber),
 new XElement(nameof(Sale.QuantityItemsRequiredtoReceivetheSale), item.QuantityItemsRequiredtoReceivetheSale),
 new XElement(nameof(Sale.FullPrice), item.FullPrice),
 new XElement(nameof(Sale.SaleOnlyforClubCustomers), item.SaleOnlyforClubCustomers),
 new XElement(nameof(Sale.SaleStartDate), item.SaleStartDate),
 new XElement(nameof(Sale.SaleEndDate), item.SaleEndDate)
 );
 root.Add(el);
 SaveX(root);
 return newId;
 }

 public Sale? Read(int id)
 {
     var root = LoadX();
     var el = root.Elements(s_elem).FirstOrDefault(x => (int?)x.Element(nameof(Sale.IDNumber)) == id);
     return el == null ? null : ParseSale(el);
 }

 public List<Sale?> ReadAll(Func<Sale, bool>? filter = null)
 {
     var root = LoadX();
     var items = root.Elements(s_elem).Select(ParseSale);
     if (filter == null) return items.Cast<Sale?>().ToList();
     return items.Where(filter).Cast<Sale?>().ToList();
 }

 public Sale? Read(Func<Sale, bool> filter)
 {
     if (filter == null) return null;
     return ReadAll(filter).FirstOrDefault();
 }

 private static Sale ParseSale(XElement el)
 {
     // המרה בטוחה מ-XElement ל-DO.Sale עם טיפול בערכים חסרים או לא תקינים
     return new Sale(
         ParseInt(el, nameof(Sale.IDNumber)),
         ParseInt(el, nameof(Sale.ProductIDNumber)),
         ParseInt(el, nameof(Sale.QuantityItemsRequiredtoReceivetheSale)),
         ParseDouble(el, nameof(Sale.FullPrice)),
         ParseBool(el, nameof(Sale.SaleOnlyforClubCustomers)),
         ParseString(el, nameof(Sale.SaleStartDate)),
         ParseString(el, nameof(Sale.SaleEndDate))
     );
 }

 private static int ParseInt(XElement el, string name)
 {
     var value = el.Element(name)?.Value;
     if (!int.TryParse(value, out var result))
         throw new InvalidDataException($"Missing or invalid '{name}' in {s_file}");
     return result;
 }

 private static double ParseDouble(XElement el, string name)
 {
     var value = el.Element(name)?.Value;
     if (!double.TryParse(value, out var result))
         throw new InvalidDataException($"Missing or invalid '{name}' in {s_file}");
     return result;
 }

 private static bool ParseBool(XElement el, string name)
 {
     var value = el.Element(name)?.Value;
     if (!bool.TryParse(value, out var result))
         throw new InvalidDataException($"Missing or invalid '{name}' in {s_file}");
     return result;
 }

 private static string ParseString(XElement el, string name)
 {
     return el.Element(name)?.Value ?? throw new InvalidDataException($"Missing '{name}' in {s_file}");
 }

 public void Update(Sale item)
 {
 if (item is null) throw new ArgumentNullException(nameof(item));
 var root = LoadX();
 var el = root.Elements(s_elem).FirstOrDefault(x => (int?)x.Element(nameof(Sale.IDNumber)) == item.IDNumber);
 if (el == null) throw new DalFacade.DalExceptions.DalDoesNotExistException($"Sale with ID {item.IDNumber} does not exist.");
 el.SetElementValue(nameof(Sale.ProductIDNumber), item.ProductIDNumber);
 el.SetElementValue(nameof(Sale.QuantityItemsRequiredtoReceivetheSale), item.QuantityItemsRequiredtoReceivetheSale);
 el.SetElementValue(nameof(Sale.FullPrice), item.FullPrice);
 el.SetElementValue(nameof(Sale.SaleOnlyforClubCustomers), item.SaleOnlyforClubCustomers);
 el.SetElementValue(nameof(Sale.SaleStartDate), item.SaleStartDate);
 el.SetElementValue(nameof(Sale.SaleEndDate), item.SaleEndDate);
 SaveX(root);
 }

 public void Delete(int id)
 {
 var root = LoadX();
 var el = root.Elements(s_elem).FirstOrDefault(x => (int?)x.Element(nameof(Sale.IDNumber)) == id);
 if (el == null) throw new DalFacade.DalExceptions.DalDoesNotExistException($"Sale with ID {id} does not exist.");
 el.Remove();
 SaveX(root);
 }
}
