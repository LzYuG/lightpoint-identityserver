// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace LightPoint.IdentityServer.Shared.Helpers
{
    public class ConsentOptionsContents
    {
        public static bool EnableOfflineAccess = true;
        public static string OfflineAccessDisplayName = "离线访问";
        public static string OfflineAccessDescription = "允许继续访问您的应用，即使您已离线";

        public static readonly string MustChooseOneErrorMessage = "您至少应该选择一项权限";
        public static readonly string InvalidSelectionErrorMessage = "异常的选择";
    }
}
