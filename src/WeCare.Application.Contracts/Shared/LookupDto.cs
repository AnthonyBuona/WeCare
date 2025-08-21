using System;

namespace WeCare.Shared;

public class LookupDto<TKey>
{
    public TKey Id { get; set; }
    public string DisplayName { get; set; }

    public LookupDto() { }

    // Novo construtor que resolve o problema!
    public LookupDto(TKey id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }


}