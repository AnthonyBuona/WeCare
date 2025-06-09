using Volo.Abp.Settings;

namespace WeCare.Settings;

public class WeCareSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WeCareSettings.MySetting1));
    }
}
