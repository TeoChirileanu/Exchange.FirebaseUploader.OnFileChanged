using System;
using FileSync;

Console.WriteLine("Calea catre fisierul urmarit:");
var fileToWatch = Console.ReadLine();

const string? credentialsFile = @"C:\Users\teodo\Documents\Google\TransactMe-1a88359a2131.json";
const string? bucketName = "transactme-db.appspot.com";

IFileUploader uploader = new FirebaseStorage(credentialsFile, bucketName);
using var _ = new ReactiveFileWatcher(fileToWatch ?? throw new InvalidOperationException(), uploader.UploadFile);

Console.Read();