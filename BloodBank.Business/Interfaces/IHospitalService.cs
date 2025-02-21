using BloodBank.Business.DTOs;

namespace BloodBank.Business.Interfaces
{

    public interface IHospitalService
    {
        Task<HospitalDto> GetHospitalByIdAsync ( int id );
        Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync ();
        Task<HospitalDto> CreateHospitalAsync ( CreateHospitalDto hospitalDto );
        Task UpdateHospitalAsync ( int id, UpdateHospitalDto hospitalDto );
        Task DeleteHospitalAsync ( int id );
        Task<IEnumerable<BloodRequestDto>> GetHospitalRequestsAsync ( int hospitalId );
        Task<HospitalDto> GetHospitalWithRequestsAsync ( int hospitalId );
        Task<bool> ValidateHospitalCredentialsAsync ( string email, string contactNumber );
    }
}
