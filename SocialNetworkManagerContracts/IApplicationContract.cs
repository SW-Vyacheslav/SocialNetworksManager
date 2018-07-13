using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworksManager.Contracts
{
    public interface IApplicationContract
    {
        void setTextBoxValue(String value);

        /// <summary>
        /// Открывает окно авторизации в браузере
        /// </summary>
        /// <param name="authpage">Ссылка на страницу авторизации</param>
        /// <returns>Возвращает токен доступа</returns>
        String openAuthWindow(String authpage);
    }
}
