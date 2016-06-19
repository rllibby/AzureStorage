/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Template10.Mvvm;

namespace AzureStorage.Models
{
    /// <summary>
    /// Enumeration for the container types.
    /// </summary>
    public enum ContainerType
    {
        /// <summary>
        /// Azure table.
        /// </summary>
        Table,

        /// <summary>
        /// Azure queue.
        /// </summary>
        Queue,

        /// <summary>
        /// Azure blob container.
        /// </summary>
        BlobContainer
    }

    /// <summary>
    /// Container class for Azure table, queue, and blob container.
    /// </summary>
    public class ResourceContainerModel : BindableBase
    {
        #region Private fields

        private readonly ResourceContainersModel _parent;
        private ContainerType _type;
        private bool _selected;
        private string _name;
        private string _uri;

        #endregion

        #region Construtor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">The owner of the item</param>
        public ResourceContainerModel(ResourceContainersModel parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            _parent = parent;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">The owner of the item</param> 
        /// <param name="type">The container type.</param>
        public ResourceContainerModel(ResourceContainersModel parent, ContainerType type) : this(parent)
        {
            _type = type;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The resource container type.
        /// </summary>
        public ContainerType ResourceType
        {
            get { return _type; }
            set
            {
                _type = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The primary uri for the resource.
        /// </summary>
        public string Uri
        {
            get { return _uri; }
            set
            {
                _uri = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The selected state of the item.
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    _parent.Selected += (_selected ? 1 : -1);

                    base.RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}
