using UnityEngine;


namespace Example
{
    public interface IBaseModel
    {
        #region Properties

        int Id { get; }
        Transform Transform { get; }

        #endregion
    }
}