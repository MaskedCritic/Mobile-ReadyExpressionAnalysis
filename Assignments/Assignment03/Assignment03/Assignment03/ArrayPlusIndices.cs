using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment03
{
    public class ArrayPlusIndices
    {
        private object[] _objects;
        private int[] _indices;

        public ArrayPlusIndices()
        {
            _objects = new object[10];
            _indices = new int[10];
        }
        public ArrayPlusIndices(object[] objects)
        {
            _objects = objects;
            _indices = new int[_objects.Length];
            for (int i = 0; i < _indices.Length; i++)
            {
                _indices[i] = i;
            }
        }

        public ArrayPlusIndices(object[] objects, int[] indices)
        {
            if (objects.Length != indices.Length)
                throw new Exception("Arrays do not match!");
            _objects = objects;
            _indices = indices;
        }

        public object[] Objeccts { get { return _objects; } }
        public int[] Indices { get { return _indices; } }
    }
}
