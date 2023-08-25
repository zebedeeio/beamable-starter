using System.Collections;
using System.Collections.Generic;
using Beamable.Common;
using Beamable.Server.Api.RealmConfig;
using UnityEngine;

public class Config
{
    private readonly IMicroserviceRealmConfigService _realmConfigService;
    private RealmConfig _settings;

    public string ApiKey => _settings.GetSetting("ZebedeeAPI", "apikey");
    public string ClientSecret => _settings.GetSetting("ZebedeeAPI", "clientsecret");
    
    public Config(IMicroserviceRealmConfigService realmConfigService)
    {
        _realmConfigService = realmConfigService;
    }

    public async Promise Init()
    {
        _settings = await _realmConfigService.GetRealmConfigSettings();
    }
}
