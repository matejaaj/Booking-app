﻿using BookingApp.Domain.Model;
using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Repository
{
    public class CheckpointRepository : ICheckpointRepository
    {
        private const string FilePath = "../../../Resources/Data/checkpoints.csv";
        private readonly Serializer<Checkpoint> _serializer;
        private List<Checkpoint> _checkpoints;

        public CheckpointRepository()
        {
            _serializer = new Serializer<Checkpoint>();
            _checkpoints = _serializer.FromCSV(FilePath); 
        }

        public List<Checkpoint> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public Checkpoint Save(Checkpoint checkpoint)
        {
            checkpoint.Id = NextId(); 
            _checkpoints = _serializer.FromCSV(FilePath);
            _checkpoints.Add(checkpoint);
            _serializer.ToCSV(FilePath, _checkpoints); 
            return checkpoint;
        }

        public int NextId()
        {
            _checkpoints = _serializer.FromCSV(FilePath);
            if (_checkpoints.Count < 1)
            {
                return 1; 
            }
            return _checkpoints.Max(c => c.Id) + 1; 
        }

        public void Delete(Checkpoint checkpoint)
        {
            _checkpoints = _serializer.FromCSV(FilePath); 
            Checkpoint toDelete = _checkpoints.Find(c => c.Id == checkpoint.Id);
            if (toDelete != null)
            {
                _checkpoints.Remove(toDelete);
                _serializer.ToCSV(FilePath, _checkpoints); 
            }
        }

        public Checkpoint Update(Checkpoint checkpoint)
        {
            _checkpoints = _serializer.FromCSV(FilePath); 
            int index = _checkpoints.FindIndex(c => c.Id == checkpoint.Id);
            if (index != -1)
            {
                _checkpoints[index] = checkpoint; 
                _serializer.ToCSV(FilePath, _checkpoints);
            }
            return checkpoint;
        }

        public Checkpoint GetById(int id)
        {
            _checkpoints = _serializer.FromCSV(FilePath);
            return _checkpoints.FirstOrDefault(checkpoint => checkpoint.Id == id);
        }
    }

}
