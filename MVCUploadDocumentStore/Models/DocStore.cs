using System;
using System.Collections.Generic;

namespace MVCUploadDocumentStore.Models;

public partial class DocStore
{
    public int DocId { get; set; }

    public string DocName { get; set; } = null!;

    public byte[] DocData { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long ContentLength { get; set; }

    public DateTime InsertionDate { get; set; }
}
