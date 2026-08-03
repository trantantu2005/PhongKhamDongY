using System;
using System.Linq;
using System.Security.Claims;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using ybaso.Blazor.Server.Services;
using ybaso.Module.BusinessObjects;

namespace ybaso.Blazor.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(
            IServiceCollection services)
        {
            /*
             * Proxy SignalR của DevExpress XAF.
             */
            services.AddSingleton(
                typeof(
                    Microsoft.AspNetCore.SignalR
                        .HubConnectionHandler<>
                ),
                typeof(ProxyHubConnectionHandler<>)
            );

            /*
             * Razor Pages và Blazor Server.
             */
            services.AddRazorPages();

            services.AddServerSideBlazor();

            /*
             * Cho phép sử dụng HttpContext.
             */
            services.AddHttpContextAccessor();

            /*
             * Cho phép sử dụng:
             *
             * [Authorize]
             * [Authorize(Roles = "BacSi")]
             */
            services.AddAuthorization();

            /*
             * Circuit Handler của DevExpress.
             */
            services.AddScoped<
                CircuitHandler,
                CircuitHandlerProxy
            >();

            /*
             * Service xác định hồ sơ bệnh nhân
             * thuộc tài khoản đang đăng nhập.
             */
            services.AddScoped<
                CurrentPatientService
            >();

            /*
             * Service cấp tài khoản đăng nhập
             * cho bệnh nhân.
             */
            services.AddScoped<
                PatientAccountService
            >();

            /*
             * Service cho phép bệnh nhân tự tạo ca khám,
             * nhập số đo và chạy phân tích.
             */
            services.AddScoped<
                PatientSelfExamService
            >();

            /*
             * Cấu hình DevExpress XAF.
             */
            services.AddXaf(
                Configuration,
                builder =>
                {
                    builder.UseApplication<
                        YbasoBlazorApplication
                    >();

                    /*
                     * Các module của ứng dụng.
                     */
                    builder.Modules
                        .AddAuditTrailXpo()
                        .AddCloning()
                        .AddConditionalAppearance()
                        .AddDashboards(options =>
                        {
                            options.DashboardDataType =
                                typeof(
                                    DevExpress
                                        .Persistent
                                        .BaseImpl
                                        .DashboardData
                                );
                        })
                        .AddFileAttachments()
                        .AddNotifications()
                        .AddOffice()
                        .AddReports(options =>
                        {
                            options.EnableInplaceReports =
                                true;

                            options.ReportDataType =
                                typeof(
                                    DevExpress
                                        .Persistent
                                        .BaseImpl
                                        .ReportDataV2
                                );

                            options.ReportStoreMode =
                                DevExpress
                                    .ExpressApp
                                    .ReportsV2
                                    .ReportStoreModes
                                    .XML;
                        })
                        .AddScheduler()
                        .AddStateMachine(options =>
                        {
                            options.StateMachineStorageType =
                                typeof(
                                    DevExpress
                                        .ExpressApp
                                        .StateMachine
                                        .Xpo
                                        .XpoStateMachine
                                );
                        })
                        .AddValidation(options =>
                        {
                            options
                                .AllowValidationDetailsAccess =
                                false;
                        })
                        .AddViewVariants()
                        .Add<
                            ybaso.Module.ybasoModule
                        >()
                        .Add<
                            ybasoBlazorModule
                        >();

                    /*
                     * Cấu hình Object Space XPO
                     * có bảo mật.
                     */
                    builder.ObjectSpaceProviders
                        .AddSecuredXpo(
                            (
                                serviceProvider,
                                options
                            ) =>
                            {
                                string connectionString =
                                    Configuration
                                        .GetConnectionString(
                                            "ConnectionString"
                                        );

#if EASYTEST
                                string easyTestConnectionString =
                                    Configuration
                                        .GetConnectionString(
                                            "EasyTestConnectionString"
                                        );

                                if (!string.IsNullOrWhiteSpace(
                                        easyTestConnectionString))
                                {
                                    connectionString =
                                        easyTestConnectionString;
                                }
#endif

                                ArgumentNullException
                                    .ThrowIfNull(
                                        connectionString
                                    );

                                options.ConnectionString =
                                    connectionString;

                                options.ThreadSafe =
                                    true;

                                options
                                    .UseSharedDataStoreProvider =
                                    true;
                            }
                        )
                        .AddNonPersistent();

                    /*
                     * Cấu hình hệ thống bảo mật XAF.
                     */
                    builder.Security
                        .UseIntegratedMode(options =>
                        {
                            options.RoleType =
                                typeof(
                                    PermissionPolicyRole
                                );

                            options.UserType =
                                typeof(
                                    ApplicationUser
                                );

                            options.UserLoginInfoType =
                                typeof(
                                    ApplicationUserLoginInfo
                                );

                            options
                                .SupportNavigationPermissionsForTypes =
                                false;

                            options
                                .UseXpoPermissionsCaching();

                            options.Events
                                .OnSecurityStrategyCreated +=
                                securityStrategy =>
                                {
                                    SecurityStrategy strategy =
                                        (SecurityStrategy)
                                        securityStrategy;

                                    /*
                                     * Luôn tải lại quyền mới nhất
                                     * sau khi Updater cập nhật.
                                     */
                                    strategy.PermissionsReloadMode =
                                        PermissionsReloadMode
                                            .NoCache;
                                };
                        })
                        .AddPasswordAuthentication(options =>
                        {
                            options
                                .IsSupportChangePassword =
                                true;

                            /*
                             * Đưa vai trò XAF vào
                             * ClaimsPrincipal của ASP.NET Core.
                             */
                            options.Events
                                .OnCustomizeLoginToken =
                                context =>
                                {
                                    using IObjectSpace objectSpace =
                                        context.ServiceProvider
                                            .GetRequiredService<
                                                INonSecuredObjectSpaceFactory
                                            >()
                                            .CreateNonSecuredObjectSpace<
                                                ApplicationUser
                                            >();

                                    if (!Guid.TryParse(
                                            context.UserId,
                                            out Guid userKey))
                                    {
                                        return;
                                    }

                                    ApplicationUser user =
                                        objectSpace
                                            .GetObjectByKey<
                                                ApplicationUser
                                            >(userKey);

                                    if (user == null)
                                    {
                                        return;
                                    }

                                    var existingRoleNames =
                                        context.Claims
                                            .Where(claim =>
                                                claim.Type ==
                                                ClaimTypes.Role
                                            )
                                            .Select(claim =>
                                                claim.Value
                                            )
                                            .ToHashSet(
                                                StringComparer
                                                    .OrdinalIgnoreCase
                                            );

                                    foreach (
                                        PermissionPolicyRole role
                                        in user.Roles
                                    )
                                    {
                                        if (role == null)
                                        {
                                            continue;
                                        }

                                        if (string.IsNullOrWhiteSpace(
                                                role.Name))
                                        {
                                            continue;
                                        }

                                        if (existingRoleNames
                                            .Contains(role.Name))
                                        {
                                            continue;
                                        }

                                        context.Claims.Add(
                                            new Claim(
                                                ClaimTypes.Role,
                                                role.Name
                                            )
                                        );

                                        existingRoleNames.Add(
                                            role.Name
                                        );
                                    }
                                };
                        });
                }
            );

            /*
             * Cấu hình Cookie Authentication.
             */
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        CookieAuthenticationDefaults
                            .AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        CookieAuthenticationDefaults
                            .AuthenticationScheme;

                    options.DefaultSignInScheme =
                        CookieAuthenticationDefaults
                            .AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    /*
                     * Trang đăng nhập XAF.
                     */
                    options.LoginPath =
                        "/LoginPage";

                    /*
                     * Trang chuyển hướng theo vai trò.
                     */
                    options.AccessDeniedPath =
                        "/role-redirect";

                    options.Cookie.HttpOnly =
                        true;

                    options.Cookie.SecurePolicy =
                        CookieSecurePolicy
                            .SameAsRequest;

                    options.SlidingExpiration =
                        true;
                });
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(
                    "/Error"
                );

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRequestLocalization();

            app.UseStaticFiles();

            /*
             * Thứ tự middleware:
             *
             * Routing
             * Authentication
             * Authorization
             * XAF
             */
            app.UseRouting();
            app.Use(async (context, next) =>
            {
                string requestPath =
                    context.Request.Path.Value ?? string.Empty;

                bool isLoginPage =
                    string.Equals(
                        requestPath.TrimEnd('/'),
                        "/LoginPage",
                        StringComparison.OrdinalIgnoreCase
                    );

                bool isGetRequest =
                    string.Equals(
                        context.Request.Method,
                        "GET",
                        StringComparison.OrdinalIgnoreCase
                    );

                if (isGetRequest && isLoginPage)
                {
                    string currentReturnUrl =
                        context.Request.Query["ReturnUrl"]
                            .ToString();

                    if (!string.Equals(
                            currentReturnUrl,
                            "/role-redirect",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Redirect(
                            "/LoginPage?ReturnUrl=%2Frole-redirect"
                        );

                        return;
                    }
                }

                await next();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseXaf();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseXaf();

            app.UseEndpoints(endpoints =>
            {
                /*
                 * Endpoint đăng xuất MediFlow.
                 *
                 * Nút đăng xuất sẽ gửi HTTP POST
                 * trực tiếp đến đường dẫn này.
                 */
                endpoints.MapPost(
                    "/mediflow-logout",
                    async context =>
                    {
                        /*
                         * Xóa cookie xác thực
                         * của scheme "Cookies".
                         */
                        await context.SignOutAsync(
                            CookieAuthenticationDefaults
                                .AuthenticationScheme
                        );

                        /*
                         * Ngăn trình duyệt lưu lại
                         * nội dung trang cũ.
                         */
                        context.Response.Headers[
                            "Cache-Control"
                        ] =
                            "no-store, no-cache";

                        context.Response.Headers[
                            "Pragma"
                        ] =
                            "no-cache";

                        /*
                         * Chuyển về trang đăng nhập XAF.
                         *
                         * Sau khi đăng nhập lại,
                         * người dùng sẽ quay về trang
                         * chuyển hướng theo vai trò.
                         */
                        context.Response.Redirect(
                            "/LoginPage" +
                            "?ReturnUrl=%2Frole-redirect"
                        );
                    }
                );

                /*
                 * Các endpoint hiện có của ứng dụng.
                 */
                endpoints.MapControllers();

                endpoints.MapXafEndpoints();

                endpoints.MapBlazorHub();

                endpoints.MapFallbackToPage(
                    "/_Host"
                );
            });
        }
    }
}