using KronoxAPI.Controller;

// See https://aka.ms/new-console-template for more information

string sessionToken = KronoxController.Login("lasse_koordt_rosenkrans.poulsen0003@stud.hkr.se", "oUPJA4j@iocd$dp", "schema.hkr.se").Result;

Console.WriteLine(KronoxController.GetUserEvents("schema.hkr.se", sessionToken).Result);