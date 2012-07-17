﻿namespace SimpleInjector.CodeSamples.Tests.Unit
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SimpleInjector.Extensions;

    [TestClass]
    public class MostResolvableParametersConstructorResolutionBehaviorTests
    {
        [TestMethod]
        public void Register_TypeWithMultipleConstructors_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            // Act
            container.Register<IDisposable, MultipleConstructorsType>();
        }

        [TestMethod]
        public void RegisterConcrete_TypeWithMultipleConstructors_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            // Act
            container.Register<MultipleConstructorsType>();
        }

        [TestMethod]
        public void GetInstance_ResolvingAConcreteUnregisteredTypeWithMultipleConstructors_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.Register<ILogger, NullLogger>();
            container.Register<ICommand, ConcreteCommand>();

            // Act
            container.GetInstance<MultipleConstructorsType>();
        }

        [TestMethod]
        public void GetInstance_ResolvingARegisteredTypeWithMultipleConstructors_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.Register<ILogger, NullLogger>();
            container.Register<ICommand, ConcreteCommand>();

            container.Register<MultipleConstructorsType>();

            // Act
            container.GetInstance<MultipleConstructorsType>();
        }

        [TestMethod]
        public void GetInstance_ResolvingARegisteredTypeWithMultipleConstructors_InjectsDependenciesUsingMostParameterConstructor()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.Register<ILogger, NullLogger>();
            container.Register<ICommand, ConcreteCommand>();

            // Act
            var instance = container.GetInstance<MultipleConstructorsType>();

            // Assert
            Assert.IsNotNull(instance.Logger, "Logger should have been injected.");
            Assert.IsNotNull(instance.Command, "Command should have been injected.");
        }

        [TestMethod]
        public void GetInstance_ResolvingARegisteredTypeWithMultipleConstructors_InjectsDependenciesUsingConstructorThatSatisfiesRegisteredDependencies1()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.Register<ILogger, NullLogger>();

            // Act
            var instance = container.GetInstance<MultipleConstructorsType>();

            // Assert
            Assert.IsNotNull(instance.Logger, "Logger should have been injected.");
            Assert.IsNull(instance.Command, "Command should not have been injected.");
        }

        [TestMethod]
        public void GetInstance_ResolvingARegisteredTypeWithMultipleConstructors_InjectsDependenciesUsingConstructorThatSatisfiesRegisteredDependencies2()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.Register<ICommand, ConcreteCommand>();

            // Act
            var instance = container.GetInstance<MultipleConstructorsType>();

            // Assert
            Assert.IsNull(instance.Logger, "Logger should not have been injected.");
            Assert.IsNotNull(instance.Command, "Command should have been injected.");
        }

        [TestMethod]
        public void RegisterOpenGeneric_GenericTypeWithMultipleConstructors_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            // Act
            container.RegisterOpenGeneric(typeof(IValidator<>), typeof(MultipleCtorNullValidator<>));
        }

        [TestMethod]
        public void GetInstance_GenericTypeWithMultipleConstructorsRegisteredWithRegisterOpenGeneric_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.RegisterOpenGeneric(typeof(IValidator<>), typeof(MultipleCtorNullValidator<>));

            container.Register<ICommand, ConcreteCommand>();

            // Act
            container.GetInstance<IValidator<int>>();
        }

        [TestMethod]
        public void RegisterManyForOpenGeneric_WithTypeContainingMultipleConstructors_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            // Act
            container.RegisterManyForOpenGeneric(typeof(IValidator<>),
                new Type[] { typeof(MultipleCtorIntValidator) });
        }

        [TestMethod]
        public void GetInstance_TypeWithMultipleConstructorsRegisteredWithRegisterManyForOpenGeneric_Succeeds()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.RegisterManyForOpenGeneric(typeof(IValidator<>),
                new Type[] { typeof(MultipleCtorIntValidator) });

            container.Register<ILogger, NullLogger>();
            container.Register<ICommand, ConcreteCommand>();

            // Act
            container.GetInstance<IValidator<int>>();
        }

        [TestMethod]
        public void GetInstance_DecoratorWithMultipleConstructors_InjectsTheRealValidatorIntoTheDecoratorsConstructorWithTheMostParameters()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            container.Register<IValidator<int>, NullValidator<int>>();

            container.RegisterDecorator(typeof(IValidator<>), typeof(MultipleCtorsValidatorDecorator<>));

            container.Register<ILogger, NullLogger>();

            // Act
            var validator = container.GetInstance<IValidator<int>>();

            // Assert
            Assert.IsInstanceOfType(validator, typeof(MultipleCtorsValidatorDecorator<int>));
            Assert.IsInstanceOfType(((MultipleCtorsValidatorDecorator<int>)validator).WrappedValidator,
                typeof(NullValidator<int>));
        }

        [TestMethod]
        public void Register_ConcreteTypeWithNoPublicConstructors_ThrowsExpectedException()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            try
            {
                // Act
                container.Register<TypeWithNoPublicConstructors>();

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ArgumentException ex)
            {
                AssertThat.StringContains("it should contain at least one public constructor.", ex.Message);
            }
        }

        [TestMethod]
        public void GetInstance_UnregisteredConcreteTypeWithNoPublicConstructors_ThrowsExpectedMessage()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            try
            {
                // Act
                container.GetInstance<TypeWithNoPublicConstructors>();

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ActivationException ex)
            {
                AssertThat.StringContains("it should contain at least one public constructor.", ex.Message);
            }
        }

        [TestMethod]
        public void GetInstance_UnregisteredConcreteTypeWithNoSelectableConstructors_ThrowsExpectedMessage()
        {
            // Arrange
            Container container = CreateContainerWithMostResolvableParametersConstructorResolutionBehavior();

            try
            {
                // Act
                // The type contains multiple public ctors, but we didn't register any needed dependencies.
                container.GetInstance<MultipleConstructorsWithSameNumberOfParametersType>();

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ActivationException ex)
            {
                AssertThat.StringContains("it should contain a public constructor that only contains " +
                    "parameters that can be resolved.", ex.Message);
            }
        }

        private static Container CreateContainerWithMostResolvableParametersConstructorResolutionBehavior()
        {
            var options = new ContainerOptions();

            var container = new Container(options);

            options.ConstructorResolutionBehavior =
                new MostResolvableParametersConstructorResolutionBehavior(container);

            return container;
        }

        public sealed class MultipleConstructorsType : IDisposable
        {
            public MultipleConstructorsType(ILogger logger)
            {
                this.Logger = logger;
            }

            public MultipleConstructorsType(ICommand command)
            {
                this.Command = command;
            }

            public MultipleConstructorsType(ILogger logger, ICommand command)
            {
                this.Logger = logger;
                this.Command = command;
            }

            public ILogger Logger { get; private set; }

            public ICommand Command { get; private set; }

            public void Dispose()
            {
            }
        }

        public sealed class MultipleConstructorsWithSameNumberOfParametersType
        {
            public MultipleConstructorsWithSameNumberOfParametersType(ICommand logger)
            {
            }

            public MultipleConstructorsWithSameNumberOfParametersType(ILogger logger)
            {
            }

            public MultipleConstructorsWithSameNumberOfParametersType(ILogger logger, ICommand command)
            {
            }

            public MultipleConstructorsWithSameNumberOfParametersType(ICommand command, ILogger logger)
            {
            }
        }

        public class MultipleCtorNullValidator<T> : IValidator<T>
        {
            public MultipleCtorNullValidator(ICommand command)
            {
            }

            public MultipleCtorNullValidator(ILogger logger, ICommand command)
            {
            }

            public void Validate(T instance)
            {
            }
        }

        public class MultipleCtorIntValidator : IValidator<int>
        {
            public MultipleCtorIntValidator(ICommand command)
            {
            }

            public MultipleCtorIntValidator(ILogger logger, ICommand command)
            {
            }

            public void Validate(int instance)
            {
            }
        }

        public class MultipleCtorsValidatorDecorator<T> : IValidator<T>
        {
            public MultipleCtorsValidatorDecorator(ICommand command)
            {
            }

            public MultipleCtorsValidatorDecorator(IValidator<T> validator, ILogger logger)
            {
                this.WrappedValidator = validator;
            }

            public IValidator<T> WrappedValidator { get; private set; }

            public void Validate(T instance)
            {
            }
        }

        public class TypeWithNoPublicConstructors
        {
            internal TypeWithNoPublicConstructors(ICommand command)
            {
            }
        }
    }
}