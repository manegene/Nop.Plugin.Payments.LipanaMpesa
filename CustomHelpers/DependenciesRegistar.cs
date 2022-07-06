using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Payments.LipanaMpesa.Models;
using Nop.Plugin.Payments.LipanaMpesa.Services;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.LipanaMpesa.CustomHelpers
{
    class DependenciesRegistar : IDependencyRegistrar
    {
        public int Order => 10;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            RegisterPluginServices(builder);

            RegisterModelBinders(builder);
        }
        private void RegisterModelBinders(ContainerBuilder builder)
        {
            builder.RegisterType<PaymentParams>().InstancePerLifetimeScope();
            builder.RegisterType<Status>().InstancePerLifetimeScope();
            builder.RegisterType<MpesaConfiguration>().InstancePerLifetimeScope();
            builder.RegisterType<MerchantModel>().InstancePerLifetimeScope();
        }
        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<Payment>().As<IPayment>().InstancePerLifetimeScope();
            builder.RegisterType<MpesaProcessPayment>().InstancePerLifetimeScope();
        }

    }
}
