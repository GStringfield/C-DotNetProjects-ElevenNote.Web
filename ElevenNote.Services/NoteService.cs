using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userId;

        public NoteService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateNote(NoteCreate model)
        {

            var entity =
                new Note()
                {
                    OwnerID = _userId,
                    Title = model.Title,
                    Content = model.Content,
                    CreateUtc = DateTimeOffset.Now
                };
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Notes.Add(entity);
                return ctx.SaveChanges() == 1;
            }







        }

        public IEnumerable<NoteListItem> GetNotes()
        {
            //temp. it will use dbcontext and then throw it away.
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx

                    .Notes
                    //like a for loop
                    .Where(e => e.OwnerID == _userId)
                    .Select(
                        e =>
                            new NoteListItem
                            {
                                NoteID = e.NoteID,
                                Title = e.Title,
                                CreatedUtc = e.CreateUtc

                            }
                    );

                return query.ToArray();
            }
        }



        public NoteDetails GetNoteById(int noteId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                     ctx
                         .Notes
                         .Single(e => e.NoteID == noteId && e.OwnerID == _userId);
                return
                    new NoteDetails
                    {
                        NoteId = entity.NoteID,
                        Title = entity.Title,
                        Content = entity.Content,
                        CreatedUtc = entity.CreateUtc,
                        ModifiedUtc = entity.ModifiedUtc

                    };



            }


        }


        public bool UpdateNote(NoteEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
             ctx
                 .Notes
                 .Single(e => e.NoteID == model.NoteId && e.OwnerID == _userId);

                entity.Title = model.Title;
                entity.Content = model.Content;
                entity.ModifiedUtc = DateTimeOffset.UtcNow;

                return ctx.SaveChanges() == 1;

            }







        }
        public bool DeleteNote(int noteId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Notes
                    .Single(e => e.NoteID == noteId && e.OwnerID == _userId);
                ctx.Notes.Remove(entity);
                return ctx.SaveChanges() == 1;

            }
        }

    }
}
