using System.Collections.Generic;
using System.Linq;
using System.IO;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;

namespace LucentDemo
{
    public static class LuceneSearch
    {
        private static string _luceneDir = @"C:\Users\dtripathi\source\repos\LucentDemo\lucene_index";
        
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }
        private static void _addToLuceneIndex(PointOfInterest interest, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("Name", interest.Name.ToString()));
            writer.DeleteDocuments(searchQuery);
            // add new index entry
            var doc = new Document();
            // add lucene fields mapped to db fields
            doc.Add(new Field("Name", interest.Name.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Type", interest.Type, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", interest.Description, Field.Store.YES, Field.Index.ANALYZED));
            // add entry to index
            writer.AddDocument(doc);
        }
        public static void AddUpdateLuceneIndex(IEnumerable<PointOfInterest> sampleDatas)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var sampleData in sampleDatas) _addToLuceneIndex(sampleData, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }
        private static PointOfInterest _mapLuceneDocumentToData(Document doc)
        {
            return new PointOfInterest
            {
                Name = doc.Get("Name"),
                Type=doc.Get("Type"),
                Description = doc.Get("Description")
            };
        }
        private static IEnumerable<PointOfInterest> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<PointOfInterest> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }
        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
        public static IEnumerable<PointOfInterest> Search
         (string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<PointOfInterest>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                    (Lucene.Net.Util.Version.LUCENE_30, new[] { "Name", "Type", "Description" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }
    }
}

