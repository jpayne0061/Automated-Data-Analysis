using CorrelationStation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Services
{
    public class DeleteRecordService
    {
        ApplicationDbContext _context;

        public DeleteRecordService()
        {
            _context = new ApplicationDbContext();
        }

        internal void DeleteRecords()
        {
            var itemsToDelete = _context.Set<KeyValue>();
            _context.KeyValues.RemoveRange(itemsToDelete);

            var itemsToDelete2 = _context.Set<ChiStats>();
            _context.ChiStats.RemoveRange(itemsToDelete2);

            var itemsToDelete3 = _context.Set<PearsonCorr>();
            _context.PearsonCorrs.RemoveRange(itemsToDelete3);

            var itemsToDelete4 = _context.Set<AnovaStats>();
            _context.AnovaStats.RemoveRange(itemsToDelete4);

            var itemsToDelete5 = _context.Set<StatSummaryVM>();
            _context.StatSummaryVMs.RemoveRange(itemsToDelete5);

            _context.SaveChanges();
        }
    }
}