using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CW11.Models;

public class Prescription_Medicament
{
    [Required]
    public int IdMedicament { get; set; }
    
    [Required]
    public int IdPrescription { get; set; }
    
    [Required]
    public int Dose { get; set; }
    
    [MaxLength(100)]
    public string Details { get; set; }
    
    [ForeignKey("IdMedicament")]
    public Medicament Medicament { get; set; }
    
    [ForeignKey("IdPrescription")]
    public Prescription Prescription { get; set; }
}