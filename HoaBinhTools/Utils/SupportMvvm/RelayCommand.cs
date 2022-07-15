
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;

namespace Utils
{
	public class RelayCommand<T> : ICommand
	{
		private readonly WeakAction<T> _execute;
		private readonly WeakFunc<T, bool> _canExecute;

		public RelayCommand(Action<T> execute, bool keepTargetAlive = false)
		  : this(execute, (Func<T, bool>)null, keepTargetAlive)
		{
		}

		public RelayCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false)
		{
			if (execute == null)
				throw new ArgumentNullException(nameof(execute));
			this._execute = new WeakAction<T>(execute, keepTargetAlive);
			if (canExecute == null)
				return;
			this._canExecute = new WeakFunc<T, bool>(canExecute, keepTargetAlive);
		}

		/// <summary>
		/// Occurs when changes occur that affect whether the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises the <see cref="E:GalaSoft.MvvmLight.Command.RelayCommand`1.CanExecuteChanged" /> event.
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			EventHandler canExecuteChanged = this.CanExecuteChanged;
			if (canExecuteChanged == null)
				return;
			canExecuteChanged((object)this, EventArgs.Empty);
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command. If the command does not require data
		/// to be passed, this object can be set to a null reference</param>
		/// <returns>true if this command can be executed; otherwise, false.</returns>
		public bool CanExecute(object parameter)
		{
			if (this._canExecute == null)
				return true;
			if (this._canExecute.IsStatic || this._canExecute.IsAlive)
			{
				if (parameter == null && typeof(T).GetTypeInfo().IsValueType)
					return this._canExecute.Execute(default(T));
				if (parameter == null || parameter is T)
					return this._canExecute.Execute((T)parameter);
			}
			return false;
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command. If the command does not require data
		/// to be passed, this object can be set to a null reference</param>
		public virtual void Execute(object parameter)
		{
			object parameter1 = parameter;
			if (!this.CanExecute(parameter1) || this._execute == null || !this._execute.IsStatic && !this._execute.IsAlive)
				return;
			if (parameter1 == null)
			{
				if (typeof(T).GetTypeInfo().IsValueType)
					this._execute.Execute(default(T));
				else
					this._execute.Execute((T)parameter1);
			}
			else
				this._execute.Execute((T)parameter1);
		}
	}


	public class RelayCommand : ICommand
	{
		private readonly WeakAction _execute;

		private readonly WeakFunc<bool> _canExecute;

		/// <summary>
		/// Initializes a new instance of the RelayCommand class that 
		/// can always execute.
		/// </summary>
		/// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
		/// you must set keepTargetAlive to true to avoid side effects. </param>
		/// <param name="keepTargetAlive">If true, the target of the Action will
		/// be kept as a hard reference, which might cause a memory leak. You should only set this
		/// parameter to true if the action is causing a closure. See
		/// http://galasoft.ch/s/mvvmweakaction. </param>
		/// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
		public RelayCommand(Action execute, bool keepTargetAlive = false)
			: this(execute, null, keepTargetAlive)
		{
		}

		/// <summary>
		/// Initializes a new instance of the RelayCommand class.
		/// </summary>
		/// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
		/// you must set keepTargetAlive to true to avoid side effects. </param>
		/// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
		/// you must set keepTargetAlive to true to avoid side effects. </param>
		/// <param name="keepTargetAlive">If true, the target of the Action will
		/// be kept as a hard reference, which might cause a memory leak. You should only set this
		/// parameter to true if the action is causing a closures. See
		/// http://galasoft.ch/s/mvvmweakaction. </param>
		/// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
		public RelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}

			_execute = new WeakAction(execute, keepTargetAlive);

			if (canExecute != null)
			{
				_canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
			}
		}

#if SILVERLIGHT
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif NETFX_CORE
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif XAMARIN
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#else
		private EventHandler _requerySuggestedLocal;

		/// <summary>
		/// Occurs when changes occur that affect whether the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
				{
					// add event handler to local handler backing field in a thread safe manner
					EventHandler handler2;
					EventHandler canExecuteChanged = _requerySuggestedLocal;

					do
					{
						handler2 = canExecuteChanged;
						EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
						canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
							ref _requerySuggestedLocal,
							handler3,
							handler2);
					}
					while (canExecuteChanged != handler2);

					CommandManager.RequerySuggested += value;
				}
			}

			remove
			{
				if (_canExecute != null)
				{
					// removes an event handler from local backing field in a thread safe manner
					EventHandler handler2;
					EventHandler canExecuteChanged = this._requerySuggestedLocal;

					do
					{
						handler2 = canExecuteChanged;
						EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value);
						canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
							ref this._requerySuggestedLocal,
							handler3,
							handler2);
					}
					while (canExecuteChanged != handler2);

					CommandManager.RequerySuggested -= value;
				}
			}
		}
#endif

		/// <summary>
		/// Raises the <see cref="CanExecuteChanged" /> event.
		/// </summary>
		[SuppressMessage(
			"Microsoft.Performance",
			"CA1822:MarkMembersAsStatic",
			Justification = "The this keyword is used in the Silverlight version")]
		[SuppressMessage(
			"Microsoft.Design",
			"CA1030:UseEventsWhereAppropriate",
			Justification = "This cannot be an event")]
		public void RaiseCanExecuteChanged()
		{
#if SILVERLIGHT
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif NETFX_CORE
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif XAMARIN
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
			CommandManager.InvalidateRequerySuggested();
#endif
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">This parameter will always be ignored.</param>
		/// <returns>true if this command can be executed; otherwise, false.</returns>
		public bool CanExecute(object parameter)
		{
			return _canExecute == null
				|| (_canExecute.IsStatic || _canExecute.IsAlive)
					&& _canExecute.Execute();
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked. 
		/// </summary>
		/// <param name="parameter">This parameter will always be ignored.</param>
		public virtual void Execute(object parameter)
		{
			if (CanExecute(parameter)
				&& _execute != null
				&& (_execute.IsStatic || _execute.IsAlive))
			{
				_execute.Execute();
			}
		}
	}

	public interface IExecuteWithObjectAndResult
	{
		/// <summary>Executes a Func and returns the result.</summary>
		/// <param name="parameter">A parameter passed as an object,
		/// to be casted to the appropriate type.</param>
		/// <returns>The result of the operation.</returns>
		object ExecuteWithObject(object parameter);
	}

	public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
	{
		private Func<T, TResult> _staticFunc;

		/// <summary>
		/// Gets or sets the name of the method that this WeakFunc represents.
		/// </summary>
		public override string MethodName
		{
			get
			{
				if (this._staticFunc != null)
					return this._staticFunc.GetMethodInfo().Name;
				return this.Method.Name;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Func's owner is still alive, or if it was collected
		/// by the Garbage Collector already.
		/// </summary>
		public override bool IsAlive
		{
			get
			{
				if (this._staticFunc == null && this.Reference == null)
					return false;
				if (this._staticFunc == null)
					return this.Reference.IsAlive;
				if (this.Reference != null)
					return this.Reference.IsAlive;
				return true;
			}
		}

		public WeakFunc(Func<T, TResult> func, bool keepTargetAlive = false)
		  : this(func == null ? (object)null : func.Target, func, keepTargetAlive)
		{
		}

		public WeakFunc(object target, Func<T, TResult> func, bool keepTargetAlive = false)
		{
			if (func.GetMethodInfo().IsStatic)
			{
				this._staticFunc = func;
				if (target == null)
					return;
				this.Reference = new WeakReference(target);
			}
			else
			{
				this.Method = func.GetMethodInfo();
				this.FuncReference = new WeakReference(func.Target);
				this.LiveReference = keepTargetAlive ? func.Target : (object)null;
				this.Reference = new WeakReference(target);
			}
		}

		/// <summary>
		/// Executes the Func. This only happens if the Func's owner
		/// is still alive. The Func's parameter is set to default(T).
		/// </summary>
		/// <returns>The result of the Func stored as reference.</returns>
		public new TResult Execute()
		{
			return this.Execute(default(T));
		}

		/// <summary>
		/// Executes the Func. This only happens if the Func's owner
		/// is still alive.
		/// </summary>
		/// <param name="parameter">A parameter to be passed to the action.</param>
		/// <returns>The result of the Func stored as reference.</returns>
		public TResult Execute(T parameter)
		{
			if (this._staticFunc != null)
				return this._staticFunc(parameter);
			object funcTarget = this.FuncTarget;
			if (!this.IsAlive || (object)this.Method == null || this.LiveReference == null && this.FuncReference == null || funcTarget == null)
				return default(TResult);
			return (TResult)this.Method.Invoke(funcTarget, new object[1]
			{
		(object) parameter
			});
		}

		/// <summary>
		/// Executes the Func with a parameter of type object. This parameter
		/// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
		/// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
		/// what type T represents.
		/// </summary>
		/// <param name="parameter">The parameter that will be passed to the Func after
		/// being casted to T.</param>
		/// <returns>The result of the execution as object, to be casted to T.</returns>
		public object ExecuteWithObject(object parameter)
		{
			return (object)this.Execute((T)parameter);
		}

		/// <summary>
		/// Sets all the funcs that this WeakFunc contains to null,
		/// which is a signal for containing objects that this WeakFunc
		/// should be deleted.
		/// </summary>
		public new void MarkForDeletion()
		{
			this._staticFunc = (Func<T, TResult>)null;
			base.MarkForDeletion();
		}
	}
	public class WeakAction
	{
		private Action _staticAction;

		/// <summary>
		/// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakAction's
		/// method passed in the constructor.
		/// </summary>
		protected MethodInfo Method { get; set; }

		/// <summary>
		/// Gets the name of the method that this WeakAction represents.
		/// </summary>
		public virtual string MethodName
		{
			get
			{
				if (this._staticAction != null)
					return this._staticAction.GetMethodInfo().Name;
				return this.Method.Name;
			}
		}

		/// <summary>
		/// Gets or sets a WeakReference to this WeakAction's action's target.
		/// This is not necessarily the same as
		/// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.Reference" />, for example if the
		/// method is anonymous.
		/// </summary>
		protected WeakReference ActionReference { get; set; }

		/// <summary>
		/// Saves the <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.ActionReference" /> as a hard reference. This is
		/// used in relation with this instance's constructor and only if
		/// the constructor's keepTargetAlive parameter is true.
		/// </summary>
		protected object LiveReference { get; set; }

		/// <summary>
		/// Gets or sets a WeakReference to the target passed when constructing
		/// the WeakAction. This is not necessarily the same as
		/// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.ActionReference" />, for example if the
		/// method is anonymous.
		/// </summary>
		protected WeakReference Reference { get; set; }

		/// <summary>
		/// Gets a value indicating whether the WeakAction is static or not.
		/// </summary>
		public bool IsStatic
		{
			get
			{
				return this._staticAction != null;
			}
		}

		/// <summary>
		/// Initializes an empty instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
		/// </summary>
		protected WeakAction()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
		/// </summary>
		/// <param name="action">The action that will be associated to this instance.</param>
		/// <param name="keepTargetAlive">If true, the target of the Action will
		/// be kept as a hard reference, which might cause a memory leak. You should only set this
		/// parameter to true if the action is using closures. See
		/// http://galasoft.ch/s/mvvmweakaction. </param>
		public WeakAction(Action action, bool keepTargetAlive = false)
		  : this(action == null ? (object)null : action.Target, action, keepTargetAlive)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
		/// </summary>
		/// <param name="target">The action's owner.</param>
		/// <param name="action">The action that will be associated to this instance.</param>
		/// <param name="keepTargetAlive">If true, the target of the Action will
		/// be kept as a hard reference, which might cause a memory leak. You should only set this
		/// parameter to true if the action is using closures. See
		/// http://galasoft.ch/s/mvvmweakaction. </param>
		public WeakAction(object target, Action action, bool keepTargetAlive = false)
		{
			if (action.GetMethodInfo().IsStatic)
			{
				this._staticAction = action;
				if (target == null)
					return;
				this.Reference = new WeakReference(target);
			}
			else
			{
				this.Method = action.GetMethodInfo();
				this.ActionReference = new WeakReference(action.Target);
				this.LiveReference = keepTargetAlive ? action.Target : (object)null;
				this.Reference = new WeakReference(target);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Action's owner is still alive, or if it was collected
		/// by the Garbage Collector already.
		/// </summary>
		public virtual bool IsAlive
		{
			get
			{
				if (this._staticAction == null && this.Reference == null && this.LiveReference == null)
					return false;
				if (this._staticAction != null)
				{
					if (this.Reference != null)
						return this.Reference.IsAlive;
					return true;
				}
				if (this.LiveReference != null)
					return true;
				if (this.Reference != null)
					return this.Reference.IsAlive;
				return false;
			}
		}

		/// <summary>
		/// Gets the Action's owner. This object is stored as a
		/// <see cref="T:System.WeakReference" />.
		/// </summary>
		public object Target
		{
			get
			{
				if (this.Reference == null)
					return (object)null;
				return this.Reference.Target;
			}
		}

		/// <summary>The target of the weak reference.</summary>
		protected object ActionTarget
		{
			get
			{
				if (this.LiveReference != null)
					return this.LiveReference;
				if (this.ActionReference == null)
					return (object)null;
				return this.ActionReference.Target;
			}
		}

		/// <summary>
		/// Executes the action. This only happens if the action's owner
		/// is still alive.
		/// </summary>
		public void Execute()
		{
			if (this._staticAction != null)
			{
				this._staticAction();
			}
			else
			{
				object actionTarget = this.ActionTarget;
				if (!this.IsAlive || (object)this.Method == null || this.LiveReference == null && this.ActionReference == null || actionTarget == null)
					return;
				this.Method.Invoke(actionTarget, (object[])null);
			}
		}

		/// <summary>Sets the reference that this instance stores to null.</summary>
		public void MarkForDeletion()
		{
			this.Reference = (WeakReference)null;
			this.ActionReference = (WeakReference)null;
			this.LiveReference = (object)null;
			this.Method = (MethodInfo)null;
			this._staticAction = (Action)null;
		}
	}

	public class WeakFunc<TResult>
	{
		private Func<TResult> _staticFunc;

		/// <summary>
		/// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakFunc's
		/// method passed in the constructor.
		/// </summary>
		protected MethodInfo Method { get; set; }

		/// <summary>
		/// Get a value indicating whether the WeakFunc is static or not.
		/// </summary>
		public bool IsStatic
		{
			get
			{
				return this._staticFunc != null;
			}
		}

		/// <summary>
		/// Gets the name of the method that this WeakFunc represents.
		/// </summary>
		public virtual string MethodName
		{
			get
			{
				if (this._staticFunc != null)
					return this._staticFunc.GetMethodInfo().Name;
				return this.Method.Name;
			}
		}

		/// <summary>
		/// Gets or sets a WeakReference to this WeakFunc's action's target.
		/// This is not necessarily the same as
		/// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.Reference" />, for example if the
		/// method is anonymous.
		/// </summary>
		protected WeakReference FuncReference { get; set; }

		/// <summary>
		/// Saves the <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference" /> as a hard reference. This is
		/// used in relation with this instance's constructor and only if
		/// the constructor's keepTargetAlive parameter is true.
		/// </summary>
		protected object LiveReference { get; set; }

		/// <summary>
		/// Gets or sets a WeakReference to the target passed when constructing
		/// the WeakFunc. This is not necessarily the same as
		/// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference" />, for example if the
		/// method is anonymous.
		/// </summary>
		protected WeakReference Reference { get; set; }

		/// <summary>Initializes an empty instance of the WeakFunc class.</summary>
		protected WeakFunc()
		{
		}

		public WeakFunc(Func<TResult> func, bool keepTargetAlive = false)
		  : this(func == null ? (object)null : func.Target, func, keepTargetAlive)
		{
		}

		public WeakFunc(object target, Func<TResult> func, bool keepTargetAlive = false)
		{
			if (func.GetMethodInfo().IsStatic)
			{
				this._staticFunc = func;
				if (target == null)
					return;
				this.Reference = new WeakReference(target);
			}
			else
			{
				this.Method = func.GetMethodInfo();
				this.FuncReference = new WeakReference(func.Target);
				this.LiveReference = keepTargetAlive ? func.Target : (object)null;
				this.Reference = new WeakReference(target);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Func's owner is still alive, or if it was collected
		/// by the Garbage Collector already.
		/// </summary>
		public virtual bool IsAlive
		{
			get
			{
				if (this._staticFunc == null && this.Reference == null && this.LiveReference == null)
					return false;
				if (this._staticFunc != null)
				{
					if (this.Reference != null)
						return this.Reference.IsAlive;
					return true;
				}
				if (this.LiveReference != null)
					return true;
				if (this.Reference != null)
					return this.Reference.IsAlive;
				return false;
			}
		}

		/// <summary>
		/// Gets the Func's owner. This object is stored as a
		/// <see cref="T:System.WeakReference" />.
		/// </summary>
		public object Target
		{
			get
			{
				if (this.Reference == null)
					return (object)null;
				return this.Reference.Target;
			}
		}

		/// <summary>
		/// Gets the owner of the Func that was passed as parameter.
		/// This is not necessarily the same as
		/// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.Target" />, for example if the
		/// method is anonymous.
		/// </summary>
		protected object FuncTarget
		{
			get
			{
				if (this.LiveReference != null)
					return this.LiveReference;
				if (this.FuncReference == null)
					return (object)null;
				return this.FuncReference.Target;
			}
		}

		/// <summary>
		/// Executes the action. This only happens if the Func's owner
		/// is still alive.
		/// </summary>
		/// <returns>The result of the Func stored as reference.</returns>
		public TResult Execute()
		{
			if (this._staticFunc != null)
				return this._staticFunc();
			object funcTarget = this.FuncTarget;
			if (this.IsAlive && (object)this.Method != null && (this.LiveReference != null || this.FuncReference != null) && funcTarget != null)
				return (TResult)this.Method.Invoke(funcTarget, (object[])null);
			return default(TResult);
		}

		/// <summary>Sets the reference that this instance stores to null.</summary>
		public void MarkForDeletion()
		{
			this.Reference = (WeakReference)null;
			this.FuncReference = (WeakReference)null;
			this.LiveReference = (object)null;
			this.Method = (MethodInfo)null;
			this._staticFunc = (Func<TResult>)null;
		}
	}


	public interface IExecuteWithObject
	{
		/// <summary>The target of the WeakAction.</summary>
		object Target { get; }

		/// <summary>Executes an action.</summary>
		/// <param name="parameter">A parameter passed as an object,
		/// to be casted to the appropriate type.</param>
		void ExecuteWithObject(object parameter);

		/// <summary>
		/// Deletes all references, which notifies the cleanup method
		/// that this entry must be deleted.
		/// </summary>
		void MarkForDeletion();
	}

	public class WeakAction<T> : WeakAction, IExecuteWithObject
	{
		private Action<T> _staticAction;

		/// <summary>
		/// Gets the name of the method that this WeakAction represents.
		/// </summary>
		public override string MethodName
		{
			get
			{
				if (this._staticAction != null)
					return this._staticAction.GetMethodInfo().Name;
				return this.Method.Name;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Action's owner is still alive, or if it was collected
		/// by the Garbage Collector already.
		/// </summary>
		public override bool IsAlive
		{
			get
			{
				if (this._staticAction == null && this.Reference == null)
					return false;
				if (this._staticAction == null)
					return this.Reference.IsAlive;
				if (this.Reference != null)
					return this.Reference.IsAlive;
				return true;
			}
		}

		public WeakAction(Action<T> action, bool keepTargetAlive = false)
		  : this(action == null ? (object)null : action.Target, action, keepTargetAlive)
		{
		}

		public WeakAction(object target, Action<T> action, bool keepTargetAlive = false)
		{
			if (action.GetMethodInfo().IsStatic)
			{
				this._staticAction = action;
				if (target == null)
					return;
				this.Reference = new WeakReference(target);
			}
			else
			{
				this.Method = action.GetMethodInfo();
				this.ActionReference = new WeakReference(action.Target);
				this.LiveReference = keepTargetAlive ? action.Target : (object)null;
				this.Reference = new WeakReference(target);
			}
		}

		/// <summary>
		/// Executes the action. This only happens if the action's owner
		/// is still alive. The action's parameter is set to default(T).
		/// </summary>
		public new void Execute()
		{
			this.Execute(default(T));
		}

		/// <summary>
		/// Executes the action. This only happens if the action's owner
		/// is still alive.
		/// </summary>
		/// <param name="parameter">A parameter to be passed to the action.</param>
		public void Execute(T parameter)
		{
			if (this._staticAction != null)
			{
				this._staticAction(parameter);
			}
			else
			{
				object actionTarget = this.ActionTarget;
				if (!this.IsAlive || (object)this.Method == null || this.LiveReference == null && this.ActionReference == null || actionTarget == null)
					return;
				this.Method.Invoke(actionTarget, new object[1]
				{
		  (object) parameter
				});
			}
		}

		/// <summary>
		/// Executes the action with a parameter of type object. This parameter
		/// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
		/// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
		/// what type T represents.
		/// </summary>
		/// <param name="parameter">The parameter that will be passed to the action after
		/// being casted to T.</param>
		public void ExecuteWithObject(object parameter)
		{
			this.Execute((T)parameter);
		}

		/// <summary>
		/// Sets all the actions that this WeakAction contains to null,
		/// which is a signal for containing objects that this WeakAction
		/// should be deleted.
		/// </summary>
		public new void MarkForDeletion()
		{
			this._staticAction = (Action<T>)null;
			base.MarkForDeletion();
		}
	}


}