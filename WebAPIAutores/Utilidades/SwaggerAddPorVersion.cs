using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAPIAutores.Utilidades
{
    public class SwaggerAddPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace;
            var versionApi = namespaceController.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
