// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.PoseModule
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Media.Media3D;
	using ConceptMatrix.Injection.Memory;

	public class Bone : INotifyPropertyChanged
	{
		public List<Bone> Children = new List<Bone>();
		public Bone Parent;

		private QuaternionMemory rotationMemory;

		public Bone(string boneName)
		{
			this.BoneName = boneName;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string BoneName { get; private set; }
		public bool IsEnabled { get; set; } = true;
		public Quaternion Rotation { get; set; }

		public string Tooltip
		{
			get
			{
				return this.BoneName; //// Strings.GetString<UISimplePoseStrings>(this.BoneName + "_Tooltip");
			}
		}

		public QuaternionMemory RotationMemory
		{
			get
			{
				if (this.rotationMemory == null)
				{
					UIntPtr address = MemoryManager.Instance.MemLib.get64bitCode(SimplePoseViewModel.GetAddressString(this.BoneName));
					this.rotationMemory = new QuaternionMemory(address);
				}

				return this.rotationMemory;
			}
		}

		public void GetRotation()
		{
			this.Rotation = this.RotationMemory.Get();
		}

		public void SetRotation()
		{
			if (!this.IsEnabled)
				return;

			if (this.RotationMemory.Value == this.Rotation)
				return;

			Quaternion newRotation = this.Rotation;

			Quaternion oldrotation = this.rotationMemory.Get();
			this.RotationMemory.Set(newRotation);
			Quaternion oldRotationConjugate = oldrotation;
			oldRotationConjugate.Conjugate();

			foreach (Bone child in this.Children)
			{
				child.Rotate(oldRotationConjugate, newRotation);
			}
		}

		private void Rotate(Quaternion sourceOldCnj, Quaternion sourceNew)
		{
			if (!this.IsEnabled)
				return;

			this.Rotation = this.RotationMemory.Get();
			Quaternion newRotation = sourceNew * (sourceOldCnj * this.Rotation);

			if (this.Rotation == newRotation)
				return;

			this.Rotation = newRotation;
			this.RotationMemory.Set(this.Rotation);

			foreach (Bone child in this.Children)
			{
				child.Rotate(sourceOldCnj, sourceNew);
			}
		}
	}
}
