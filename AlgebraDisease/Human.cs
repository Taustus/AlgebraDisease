using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraDisease
{
    public enum DiseaseForm
    {
        Heavy,
        Light,
        Healthy
    }

    class Human
    {
        bool HeavyForm { get; set; }

        bool LightForm { get; set; }

        public DiseaseForm CurrentState { get; set; }

        public Human()
        {
            CurrentState = DiseaseForm.Healthy;
            LightForm = false;
            HeavyForm = false;
        }

        void GetSickInHeavyForm()
        {
            if (LightForm)
            {
                throw new DiseaseException("Already sick in lightform!");
            }
            else
            {
                HeavyForm = true;
                CurrentState = DiseaseForm.Heavy;
            }
        }

        void GetSickInLightForm()
        {
            if(HeavyForm)
            {
                throw new DiseaseException("Already sick in heavyform!");
            }
            else
            {
                LightForm = true;
                CurrentState = DiseaseForm.Light;
            }
        }

        public void GetSickerForOneLevel()
        {
            if(!LightForm && !HeavyForm)
            {
                GetSickInLightForm();
            }
            else if(LightForm)
            {
                LightForm = false;
                GetSickInHeavyForm();
            }
            else
            {
                throw new DiseaseException("Can't get any sicker!");
            }
        }

        public void GetWellForOneLevel()
        {
            if(HeavyForm)
            {
                HeavyForm = false;
                LightForm = true;
                CurrentState = DiseaseForm.Light;
            }
            else if(LightForm)
            {
                LightForm = false;
                CurrentState = DiseaseForm.Healthy;
            }
            else
            {
                throw new DiseaseException("Already healthy!");
            }
        }
    }

    class DiseaseException : Exception
    {
        public DiseaseException(string message) : base(message)
        {

        }
    }
}
