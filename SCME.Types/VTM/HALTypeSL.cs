using System.Runtime.Serialization;

namespace SCME.Types.VTM
{
    /// <summary>Тип тестирования SL</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum VTMTestType
    {
        /// <summary>Трапеция</summary>
        [EnumMember]
        Ramp,
        /// <summary>Полусинус</summary>
        [EnumMember]
        Sinus,
        /// <summary>S-кривая</summary>
        [EnumMember]
        Curve
    };
}